﻿using Microsoft.Z3;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Policy;

namespace Z3.LinqBinding
{

    public enum Optimization
    {
        Maximize,
        Minimize
    }


    /// <summary>
    /// Representation of a theorem with its constraints.
    /// </summary>
    public class Theorem
    {
        /// <summary>
        /// Theorem constraints.
        /// </summary>
        private IEnumerable<LambdaExpression> _constraints;

        /// <summary>
        /// Z3 context under which the theorem is solved.
        /// </summary>
        private Z3Context _context;

        /// <summary>
        /// Creates a new theorem for the given Z3 context.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        protected Theorem(Z3Context context)
            : this(context, new List<LambdaExpression>())
        {
        }

        /// <summary>
        /// Creates a new pre-constrained theorem for the given Z3 context.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="constraints">Constraints to apply to the created theorem.</param>
        protected Theorem(Z3Context context, IEnumerable<LambdaExpression> constraints)
        {
            _context = context;
            _constraints = constraints;
        }

        /// <summary>
        /// Gets the constraints of the theorem.
        /// </summary>
        protected IEnumerable<LambdaExpression> Constraints
        {
            get
            {
                return _constraints;
            }
        }

        /// <summary>
        /// Gets the Z3 context under which the theorem is solved.
        /// </summary>
        protected Z3Context Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Returns a comma-separated representation of the constraints embodied in the theorem.
        /// </summary>
        /// <returns>Comma-separated string representation of the theorem's constraints.</returns>
        public override string ToString()
        {
            return string.Join(", ", (from c in _constraints select c.Body.ToString()).ToArray());
        }

        /// <summary>
        /// Solves the theorem using Z3.
        /// </summary>
        /// <typeparam name="T">Theorem environment type.</typeparam>
        /// <returns>Result of solving the theorem; default(T) if the theorem cannot be satisfied.</returns>
        protected T Solve<T>()
        {
            // TODO: some debugging around issues with proper disposal of native resources…
            // using (Context context = _context.CreateContext())
            Context context = _context.CreateContext();
            {
                var environment = GetEnvironment<T>(context);
                //Solver solver = context.MkSimpleSolver();
                Solver solver = context.MkSolver();


                AssertConstraints<T>(context, solver, environment);

                //Model model = null;
                //if (context.CheckAndGetModel(ref model) != LBool.True)
                //    return default(T);

                Status status = solver.Check();
                if (status != Status.SATISFIABLE)
                {
                    return default(T);
                }

                return GetSolution<T>(context, solver.Model, environment);
            }
        }

        /// <summary>
        /// Solves the theorem using Z3.
        /// </summary>
        /// <typeparam name="T">Theorem environment type.</typeparam>
        /// <returns>Result of solving the theorem; default(T) if the theorem cannot be satisfied.</returns>
        protected T Optimize<T>(Optimization direction, Expression<Func<T, int>> lambda)
        {

            Context context = _context.CreateContext();
            {
                var environment = GetEnvironment<T>(context);
                //Solver solver = context.MkSimpleSolver();
                Optimize optimizer = context.MkOptimize();


                AssertConstraints<T>(context, optimizer, environment);


                var exp = Visit(context, environment, lambda.Body, lambda.Parameters[0]);
                switch (direction)
                {
                    case Optimization.Maximize:
                        optimizer.MkMaximize(exp);
                        break;
                    case Optimization.Minimize:
                        optimizer.MkMinimize(exp);
                        break;
                }


                Status status = optimizer.Check();
                if (status != Status.SATISFIABLE)
                {
                    return default(T);
                }

                return GetSolution<T>(context, optimizer.Model, environment);
            }
        }


        /// <summary>
        /// Maps the properties on the theorem environment type to Z3 handles for bound variables.
        /// </summary>
        /// <typeparam name="T">Theorem environment type to create a mapping table for.</typeparam>
        /// <param name="context">Z3 context.</param>
        /// <returns>Environment mapping table from .NET properties onto Z3 handles.</returns>
        private static Dictionary<PropertyInfo, Expr> GetEnvironment<T>(Context context)
        {
            var environment = new Dictionary<PropertyInfo, Expr>();

            //
            // All public properties are considered part of the theorem's environment.
            // Notice we can't require custom attribute tagging if we want the user to be able to
            // use anonymous types as a convenience solution.
            //
            foreach (var parameter in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                //
                // Normalize types when facing Z3. Theorem variable type mappings allow for strong
                // typing within the theorem, while underlying variable representations are Z3-
                // friendly types.
                //
                var parameterType = parameter.PropertyType;
                var parameterTypeMapping = (TheoremVariableTypeMappingAttribute)parameterType.GetCustomAttributes(typeof(TheoremVariableTypeMappingAttribute), false).SingleOrDefault();
                if (parameterTypeMapping != null)
                    parameterType = parameterTypeMapping.RegularType;

                //
                // Map the environment onto Z3-compatible types.
                //
                if (parameterType == typeof(bool))
                    //environment.Add(parameter, context.MkConst(parameter.Name, context.MkBoolType()));
                    environment.Add(parameter, context.MkBoolConst(parameter.Name));
                else if (parameterType == typeof(int))
                    //environment.Add(parameter, context.MkConst(parameter.Name, context.MkIntType()));
                    environment.Add(parameter, context.MkIntConst(parameter.Name));
                else if (parameterType.IsArray) //(typeof(IEnumerable).IsAssignableFrom(parameterType))
                {
                    Sort arrDomain;
                    Sort arrRange;
                    switch (Type.GetTypeCode(parameterType.GetElementType()))
                    {
                        case TypeCode.Int16:
                            arrDomain = context.IntSort;
                            arrRange = context.MkBitVecSort(16);
                            break;
                        case TypeCode.Int32:
                            arrDomain = context.IntSort;
                            arrRange = context.IntSort;
                            break;
                        case TypeCode.Int64:
                            arrDomain = context.IntSort;
                            arrRange = context.MkBitVecSort(64);
                            break;
                        case TypeCode.Boolean:
                            arrDomain = context.BoolSort;
                            arrRange = context.BoolSort;
                            break;
                        case TypeCode.Single:
                            arrDomain = context.RealSort;
                            arrRange = context.RealSort;
                            break;
                        case TypeCode.Double:
                            arrDomain = context.RealSort;
                            arrRange = context.MkBitVecSort(64);
                            break;
                        default:
                            throw new NotSupportedException("Unsupported parameter type for " + parameter.Name + ".");

                    }
                    environment.Add(parameter, context.MkArrayConst(parameter.Name, arrDomain, arrRange));
                }
                else
                    throw new NotSupportedException("Unsupported parameter type for " + parameter.Name + ".");
            }

            return environment;
        }

        /// <summary>
        /// Gets the solution object for the solved theorem.
        /// </summary>
        /// <typeparam name="T">Environment type to create an instance of.</typeparam>
        /// <param name="model">Z3 model to evaluate theorem parameters under.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <returns>Instance of the enviroment type with theorem-satisfying values.</returns>
        private static T GetSolution<T>(Context context, Model model, Dictionary<PropertyInfo, Expr> environment)
        {
            Type t = typeof(T);

            //
            // Determine whether T is a compiler-generated type, indicating an anonymous type.
            // This check might not be reliable enough but works for now.
            //
            if (t.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any())
            {
                //
                // Anonymous types have a constructor that takes in values for all its properties.
                // However, we don't know the order and it's hard to correlate back the parameters
                // to the underlying properties. So, we want to bypass that constructor altogether
                // by using the FormatterServices to create an uninitialized (all-zero) instance.
                //
                T result = (T)FormatterServices.GetUninitializedObject(t);

                //
                // Here we take advantage of undesirable knowledge on how anonymous types are
                // implemented by the C# compiler. This is risky but we can live with it for
                // now in this POC. Because the properties are get-only, we need to perform
                // nominal matching with the corresponding backing fields.
                //
                var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var parameter in environment.Keys)
                {
                    //
                    // Mapping from property to field.
                    //
                    var field = fields.Where(f => f.Name.StartsWith("<" + parameter.Name + ">")).SingleOrDefault();

                    //
                    // Evaluation of the values though the handle in the environment bindings.
                    //
                    Expr val = model.Eval(environment[parameter]);
                    if (parameter.PropertyType == typeof(bool))
                        field.SetValue(result, val.IsTrue);
                    else if (parameter.PropertyType == typeof(int))
                        field.SetValue(result, ((IntNum)val).Int);
                    else
                        throw new NotSupportedException("Unsupported parameter type for " + parameter.Name + ".");
                }

                return result;
            }
            else
            {
                //
                // Straightforward case of having an "onymous type" at hand.
                //
                T result = Activator.CreateInstance<T>();

                foreach (var parameter in environment.Keys)
                {
                    //
                    // Normalize types when facing Z3. Theorem variable type mappings allow for strong
                    // typing within the theorem, while underlying variable representations are Z3-
                    // friendly types.
                    //
                    var parameterType = parameter.PropertyType;
                    var parameterTypeMapping = (TheoremVariableTypeMappingAttribute)parameterType.GetCustomAttributes(typeof(TheoremVariableTypeMappingAttribute), false).SingleOrDefault();
                    if (parameterTypeMapping != null)
                        parameterType = parameterTypeMapping.RegularType;

                    //
                    // Evaluation of the values though the handle in the environment bindings.
                    //
                    Expr val = model.Eval(environment[parameter]);
                    object value;
                    if (parameterType == typeof(bool))
                        value = val.IsTrue;
                    else if (parameterType == typeof(int))
                        value = ((IntNum)val).Int;
                    else if (parameterType == typeof(long))
                        value = ((IntNum)val).Int64;
                    else if (parameterType == typeof(Single))
                        value = Double.Parse(((RatNum)val).ToDecimalString(32), CultureInfo.InvariantCulture);
                    else if (parameterType == typeof(double))
                        value = Double.Parse(((RatNum)val).ToDecimalString(64), CultureInfo.InvariantCulture);
                    else if (parameterType.IsArray)
                    {
                        var eltType = parameterType.GetElementType();
                        if (eltType == null)
                        {
                            throw new NotSupportedException("Unsupported untyped array parameter type for " + parameter.Name + ".");
                        }
                        var arrVal = (ArrayExpr)environment[parameter];

                        //var arrVal = (Quantifier)val;

                        var results = new ArrayList();
                        var existingLength = ((Array)parameter.GetValue(result, null)).Length;
                        for (int i = 0; i < existingLength; i++)
                        {
                            var numValExpr = model.Eval(context.MkSelect(arrVal, context.MkInt(i)));
                            object numVal;
                            if (eltType == typeof(bool))
                                numVal = numValExpr.IsTrue;
                            else if (eltType == typeof(int))
                                numVal = ((IntNum)numValExpr).Int;
                            else if (eltType == typeof(long))
                                numVal = ((IntNum)numValExpr).Int64;
                            else if (eltType == typeof(Single))
                                numVal = Double.Parse(((RatNum)numValExpr).ToDecimalString(32), CultureInfo.InvariantCulture);
                            else if (eltType == typeof(double))
                                numVal = Double.Parse(((RatNum)numValExpr).ToDecimalString(64), CultureInfo.InvariantCulture);
                            else
                                throw new NotSupportedException($"Unsupported array parameter type for {parameter.Name} and array element type {eltType.Name}.");
                            results.Add(numVal);
                        }
                        value = results.ToArray(eltType);
                    }
                    else
                        throw new NotSupportedException("Unsupported parameter type for " + parameter.Name + ".");

                    //
                    // If there was a type mapping, we need to convert back to the original type.
                    // In that case we expect a constructor with the mapped type to be available.
                    //
                    if (parameterTypeMapping != null)
                    {
                        var ctor = parameter.PropertyType.GetConstructor(new Type[] { parameterType });
                        if (ctor == null)
                            throw new InvalidOperationException("Could not construct an instance of the mapped type " + parameter.PropertyType.Name + ". No public constructor with parameter type " + parameterType + " found.");

                        value = ctor.Invoke(new object[] { value });
                    }

                    parameter.SetValue(result, value, null);
                }

                return result;
            }
        }


        /// <summary>
        /// Main visitor method to translate the LINQ expression tree into a Z3 expression handle.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <param name="expression">LINQ expression tree node to be translated.</param>
        /// <param name="param">Parameter used to express the constraint on.</param>
        /// <returns>Z3 expression handle.</returns>
        private Expr Visit(Context context, Dictionary<PropertyInfo, Expr> environment, Expression expression, ParameterExpression param)
        {
            //
            // Largely table-driven mechanism, providing constructor lambdas to generic Visit*
            // methods, classified by type and arity.
            //
            switch (expression.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkAnd((BoolExpr)a, (BoolExpr)b));

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkOr((BoolExpr)a, (BoolExpr)b));

                case ExpressionType.ExclusiveOr:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkXor((BoolExpr)a, (BoolExpr)b));

                case ExpressionType.Not:
                    return VisitUnary(context, environment, (UnaryExpression)expression, param, (ctx, a) => ctx.MkNot((BoolExpr)a));

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return VisitUnary(context, environment, (UnaryExpression)expression, param, (ctx, a) => ctx.MkUnaryMinus((ArithExpr)a));

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkAdd((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkSub((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkMul((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.Divide:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkDiv((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.Modulo:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkRem((IntExpr)a, (IntExpr)b));

                case ExpressionType.LessThan:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkLt((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.LessThanOrEqual:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkLe((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.GreaterThan:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkGt((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.GreaterThanOrEqual:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkGe((ArithExpr)a, (ArithExpr)b));

                case ExpressionType.Equal:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkEq(a, b));

                case ExpressionType.NotEqual:
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkNot(ctx.MkEq(a, b)));

                case ExpressionType.MemberAccess:
                    return VisitMember(context, environment, (MemberExpression)expression, param);

                case ExpressionType.Constant:
                    return VisitConstant(context, (ConstantExpression)expression);

                case ExpressionType.Call:
                    return VisitCall(context, environment, (MethodCallExpression)expression, param);
                case ExpressionType.ArrayIndex:
                    //return VisitSingleArray(context, environment, (IndexExpression)expression, param, (ctx, a, b) => ctx.MkSelect(a, b));
                    return VisitBinary(context, environment, (BinaryExpression)expression, param, (ctx, a, b) => ctx.MkSelect((ArrayExpr)a, b));

                default:
                    throw new NotSupportedException("Unsupported expression node type encountered: " + expression.NodeType);
            }
        }

        /// <summary>
        /// Visitor method to translate a constant expression.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="constant">Constant expression.</param>
        /// <returns>Z3 expression handle.</returns>
        private static Expr VisitConstant(Context context, ConstantExpression constant)
        {
            if (constant.Type == typeof(int))
                //return context.MkNumeral((int)constant.Value, context.MkIntType());
                return context.MkNumeral((int)constant.Value, context.IntSort);
            else if (constant.Type == typeof(bool))
                return (bool)constant.Value ? context.MkTrue() : context.MkFalse();

            throw new NotSupportedException("Unsupported constant type.");
        }

        /// <summary>
        /// Visitor method to translate a member expression.
        /// </summary>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <param name="member">Member expression.</param>
        /// <param name="param">Parameter used to express the constraint on.</param>
        /// <returns>Z3 expression handle.</returns>
        private static Expr VisitMember(Context context, Dictionary<PropertyInfo, Expr> environment, MemberExpression member, ParameterExpression param)
        {
            //
            // E.g. Symbols l = ...;
            //      theorem.Where(s => l.X1)
            //                         ^^
            //
            if (member.Expression != param)
            {
                //throw new NotSupportedException("Encountered member access not targeting the constraint parameter.");
                if (typeof(ConstantExpression).IsInstanceOfType(member.Expression))
                {
                    var target = ((ConstantExpression)member.Expression).Value;
                    object val = null;
                    switch (member.Member.MemberType)
                    {
                        case MemberTypes.Property:
                            val = ((PropertyInfo)member.Member).GetValue(target, null);
                            break;
                        case MemberTypes.Field:
                            val = ((FieldInfo)member.Member).GetValue(target);
                            break;
                        default:
                            //val = target = null;
                            throw new NotSupportedException($"Unsupported constant {target} .");

                    }

                    if (val != null)
                    {
                        switch (Type.GetTypeCode(val.GetType()))
                        {
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                                return context.MkInt(Convert.ToInt64(val));
                            case TypeCode.Boolean:
                                return context.MkBool((bool)val);
                            case TypeCode.Single:
                            case TypeCode.Double:
                                return context.MkReal(val.ToString());
                            default:
                                throw new NotSupportedException($"Unsupported constant {val} .");

                        }
                    }




                }
                throw new NotSupportedException("Encountered member access not targeting the constraint parameter.");
            }


            //
            // Only members we allow currently are direct accesses to the theorem's variables
            // in the environment type. So we just try to find the mapping from the environment
            // bindings table.
            //
            PropertyInfo property;
            Expr value;
            if ((property = member.Member as PropertyInfo) == null
                || !environment.TryGetValue(property, out value))
                throw new NotSupportedException("Unknown parameter encountered: " + member.Member.Name + ".");

            return value;
        }

        /// <summary>
        /// Asserts the theorem constraints on the Z3 context.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <typeparam name="T">Theorem environment type.</typeparam>
        private void AssertConstraints<T>(Context context, Object solverOrOptimizer, Dictionary<PropertyInfo, Expr> environment)
        {
            var constraints = _constraints;

            //
            // Global rewriter registered?
            //
            var rewriterAttr = (TheoremGlobalRewriterAttribute)typeof(T).GetCustomAttributes(typeof(TheoremGlobalRewriterAttribute), false).SingleOrDefault();
            if (rewriterAttr != null)
            {
                //
                // Make sure the specified rewriter type implements the ITheoremGlobalRewriter.
                //
                var rewriterType = rewriterAttr.RewriterType;
                if (!typeof(ITheoremGlobalRewriter).IsAssignableFrom(rewriterType))
                    throw new InvalidOperationException("Invalid global rewriter type definition. Did you implement ITheoremGlobalRewriter?");

                //
                // Assume a parameterless public constructor to new up the rewriter.
                //
                var rewriter = (ITheoremGlobalRewriter)Activator.CreateInstance(rewriterType);

                //
                // Do the rewrite.
                //
                constraints = rewriter.Rewrite(constraints);
            }

            //
            // Visit, assert and log.
            //
            foreach (var constraint in constraints)
            {
                BoolExpr c = (BoolExpr)Visit(context, environment, constraint.Body, constraint.Parameters[0]);

                //context.AssertCnstr(c);
                if (solverOrOptimizer is Solver solver)
                {
                    solver.Assert(c);
                }
                else
                {
                    if (solverOrOptimizer is Optimize optimizer)
                    {
                        optimizer.Assert(c);
                    }
                }


                //_context.LogWriteLine(context.ToString(c));
                _context.LogWriteLine(c.ToString());
            }
        }



        /// <summary>
        /// Visitor method to translate a binary expression.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <param name="expression">Binary expression.</param>
        /// <param name="ctor">Constructor to combine recursive visitor results.</param>
        /// <param name="param">Parameter used to express the constraint on.</param>
        /// <returns>Z3 expression handle.</returns>
        private Expr VisitBinary(Context context, Dictionary<PropertyInfo, Expr> environment, BinaryExpression expression, ParameterExpression param, Func<Context, Expr, Expr, Expr> ctor)
        {
            return ctor(context, Visit(context, environment, expression.Left, param), Visit(context, environment, expression.Right, param));
        }

        /// <summary>
        /// Visitor method to translate a method call expression.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <param name="call">Method call expression.</param>
        /// <param name="param">Parameter used to express the constraint on.</param>
        /// <returns>Z3 expression handle.</returns>
        private Expr VisitCall(Context context, Dictionary<PropertyInfo, Expr> environment, MethodCallExpression call, ParameterExpression param)
        {
            var method = call.Method;

            //
            // Does the method have a rewriter attribute applied?
            //
            var rewriterAttr = (TheoremPredicateRewriterAttribute)method.GetCustomAttributes(typeof(TheoremPredicateRewriterAttribute), false).SingleOrDefault();
            if (rewriterAttr != null)
            {
                //
                // Make sure the specified rewriter type implements the ITheoremPredicateRewriter.
                //
                var rewriterType = rewriterAttr.RewriterType;
                if (!typeof(ITheoremPredicateRewriter).IsAssignableFrom(rewriterType))
                    throw new InvalidOperationException("Invalid predicate rewriter type definition. Did you implement ITheoremPredicateRewriter?");

                //
                // Assume a parameterless public constructor to new up the rewriter.
                //
                var rewriter = (ITheoremPredicateRewriter)Activator.CreateInstance(rewriterType);

                //
                // Make sure we don't get stuck when the rewriter just returned its input. Valid
                // rewriters should satisfy progress guarantees.
                //
                var result = rewriter.Rewrite(call);
                if (result == call)
                    throw new InvalidOperationException("The expression tree rewriter of type " + rewriterType.Name + " did not perform any rewrite. Aborting compilation to avoid infinite looping.");

                //
                // Visit the rewritten expression.
                //
                return Visit(context, environment, result, param);
            }

            //
            // Filter for known Z3 operators.
            //
            if (method.IsGenericMethod && method.GetGenericMethodDefinition() == typeof(Z3Methods).GetMethod("Distinct"))
            {
                //
                // We know the signature of the Distinct method call. Its argument is a params
                // array, hence we expect a NewArrayExpression.
                //

                var valType = method.GetGenericArguments()[0];

                NewArrayExpression arr = null;
                if (call.Arguments[0] is NewArrayExpression arrExp)
                {
                    arr = arrExp;
                }
                else
                {
                    throw new NotSupportedException("unsuported method call:" + method.ToString() + "with sub expression " + call.Arguments[0].ToString());
                    //Debugger.Break();
                    //IEnumerable<Expression> result =  Expression.Lambda(call.Arguments[0]).Compile();
                    //arr = Expression.NewArrayInit(valType, result);
                }

                var args = from arg in arr.Expressions select Visit(context, environment, arg, param);
                return context.MkDistinct(args.ToArray());
            }
            else
                throw new NotSupportedException("Unknown method call:" + method.ToString());
        }

        /// <summary>
        /// Visitor method to translate a unary expression.
        /// </summary>
        /// <param name="context">Z3 context.</param>
        /// <param name="environment">Environment with bindings of theorem variables to Z3 handles.</param>
        /// <param name="expression">Unary expression.</param>
        /// <param name="ctor">Constructor to combine recursive visitor results.</param>
        /// <param name="param">Parameter used to express the constraint on.</param>
        /// <returns>Z3 expression handle.</returns>
        private Expr VisitUnary(Context context, Dictionary<PropertyInfo, Expr> environment, UnaryExpression expression, ParameterExpression param, Func<Context, Expr, Expr> ctor)
        {
            return ctor(context, Visit(context, environment, expression.Operand, param));
        }

    }
}