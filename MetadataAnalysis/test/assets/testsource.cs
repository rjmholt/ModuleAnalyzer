using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleTestAttribute : Attribute
{
}

public class PositionalTestAttribute : Attribute
{
    public PositionalTestAttribute(int intArg, double doubleArg)
    {
        IntArg = intArg;
        DoubleArg = doubleArg;
    }

    public int IntArg { get; }

    public double DoubleArg { get; }
}

public class Quux
{
    
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class NamedTestAttribute : Attribute
{
    public NamedTestAttribute()
    {
        IntArg = 14;
        StringArg = "DEFAULT";
    }

    public int IntArg { get; set; }

    public string StringArg { get; set; }
}

public class CombinedTestAttribute : Attribute
{
    public CombinedTestAttribute(int intArg)
    {
        StringArg = "DEFAULT";
    }

    public string StringArg { get; set; }
}

public class TestObject
{
    #region Constructors

    public TestObject() {}

    protected TestObject(Quux arg1, List<string> arg2) {}

    private TestObject(string arg = "DEFAULT") {}

    internal TestObject(params object[] args) {}

    #endregion /* Constructors */

    #region Members

    public DateTime dateTimeMember;

    internal List<int> intListMember;

    protected object objectMember;

    private Quux quuxMember;

    Exception exceptionMember;

    #endregion /* Members */

    #region Properties

    public string StringProperty { get; set; }

    internal int IntProperty { get; set; }

    protected IDictionary<string, object> IDictionaryProperty { get; }

    private Stack StackProperty { get; }

    #endregion /* Properties */

    #region Methods

    public int IntMethod() { return 7; }

    internal string StringMethod() { return "STRING"; }

    protected DateTime DateTimeMethod() { return DateTime.Now; }

    private Quux QuuxMethod() { return null; }

    public double OneArgumentMethod(double x)
    {
        return x;
    }

    public DateTime TwoArgumentMethod(DateTime t, double delta)
    {
        return t.AddHours(delta);
    }

    public object VarargsMethod(params object[] args)
    {
        return args[0];
    }

    public void VoidMethod(string s)
    {
        Console.WriteLine(s);
    }

    #endregion /* Methods */

    #region Custom Attributes

    [SimpleTest]
    public TestObject(int arg)
    {
    }

    [SimpleTest]
    public string simpleAttributeMember;

    [SimpleTest]
    public string SimpleAttributeProperty { get; set; }

    [SimpleTest]
    public void SimpleAttributeMethod()
    {
    }

    [PositionalTest(3, 7.2)]
    public void PositionalAttributeMethod()
    {
    }

    [NamedTest()]
    public void DefaultNamedAttributeMethod()
    {
    }

    [NamedTest(IntArg = 10)]
    public void PartialNamedAttributeMethod()
    {
    }

    [NamedTest(IntArg = 20, StringArg = "NEWVALUE")]
    public void FullNamedAttributeMethod()
    {
    }

    [NamedTest(), NamedTest(IntArg = 19)]
    public void MultiNamedAttributeMethod()
    {
    }

    [CombinedTest(34)]
    public void PartialCombinedAttributeMethod()
    {
    }

    [CombinedTest(65, StringArg = "NEWVALUE")]
    public void FullCombinedAttributeMethod()
    {
    }

    [Obsolete]
    [PositionalTest(3, 8)]
    [NamedTest(), NamedTest(StringArg = "NEWVALUE")]
    public void MultiAttributeMethod()
    {
    }

    [Obsolete]
    public void ObsoleteAttributeMethod()
    {
    }

    #endregion /* Custom Attributes */

    #region Constants

    public const int INT_CONST = 10;

    internal const string STR_CONST = "DEFAULT";

    protected const double DOUBLE_CONST = 8.3;

    private const float FLOAT_CONST = 5f;

    #endregion /* Constants */

    #region Static Constructor

    static TestObject() {}

    #endregion /* Static Constructor */

    #region Static Members

    public static object s_object;

    internal static Quux s_quux;

    protected static Stack s_stack;

    private int s_int = 10;

    #endregion /* Static Members */

    #region Static Properties

    public static string StaticStringProperty { get; set; }

    internal static Quux StaticQuuxProperty { get; }

    protected static List<DateTime> StaticDateTimeListProperty { get; set; }

    private static int StaticIntProperty { get; set; }

    #endregion /* Static Properties */

    #region Static Methods

    public static void StaticVoidNoArgMethod() {}

    internal static int StaticOneArgMethod(int x) { return x; }

    protected static Quux StaticTwoArgMethod(string arg1, double arg2)
    {
        return new Quux();
    }

    private static List<Quux> StaticVarargsMethod(params string[] args)
    {
        return null;
    }

    #endregion /* Static Methods */
}

internal static class StaticObject
{
}

public class InheritanceObject : Quux, ICloneable
{
    public object Clone()
    {
        throw new NotImplementedException();
    }
}

public class NestingObject
{
    public class PublicInnerObject
    {
        public class PublicInnerObject2
        {
            public class PublicInnerObject3_1
            {
                public class PublicInnerObject4
                {
                }
            }

            public class PublicInnerObject3_2
            {
            }
        }
    }

    internal class InternalInnerObject
    {
    }

    protected class ProtectedInnerObject
    {
    }

    private class PrivateInnerObject
    {
    }
}

[PositionalTest(3, 4.2)]
[NamedTest()]
[Serializable]
public class AttributeObject
{
}