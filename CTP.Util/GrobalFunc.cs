using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CTP.Util
{
    public static class GrobalFunc
    {
        #region Ascii 转换

        private static String PREFIX = "\\u";
        public static String native2Ascii(this String str)
        {
            return native2Ascii(str, false);
        }

        /// <summary>
        /// 是否加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="isSerc"></param>
        /// <returns></returns>
        public static String native2Ascii(this String str, Boolean isSerc)
        {
            if (!isSerc)
            {
                return str;
            }

            char[] chars = str.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                sb.Append(char2Ascii(chars[i]));
            }
            return sb.ToString();
        }

        private static String char2Ascii(char c)
        {
            if (c > 255)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(PREFIX);
                int code = (c >> 8);
                String tmp = code.ToString("X");
                if (tmp.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(tmp);
                code = (c & 0xFF);
                tmp = code.ToString("X");
                if (tmp.Length == 1)
                {
                    sb.Append("0");
                }
                sb.Append(tmp);
                return sb.ToString();
            }
            else
            {
                return c.ToString();
            }
        }

        #endregion

        #region 序列化Json字符串,生成查询字符串,Redis不规则数据生成Json

        public static string ToJson<T>(this T value) where T : class
        {
            string json = string.Empty;
            IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            json = JsonConvert.SerializeObject(value, dtConverter);
            return JsonConvert.SerializeObject(value);
        }

        public static string ToQueryCondition<T>(this T value) where T : class
        {
            string json = string.Empty;
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            setting.DefaultValueHandling = DefaultValueHandling.Ignore;
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            json = JsonConvert.SerializeObject(value, setting);
            json = json.Replace(",", "*").Substring(1, json.Length - 1);
            return json.Substring(0, json.Length - 1);
        }

        /// <summary>
        /// Redis 数据 处理成符合Json
        /// </summary>
        public static string RedisDataToJson(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            string v = value.Replace("[\"{", "[{").Replace("}\"]", "}]").Replace("\\\"", "\"").Replace("\"Result\":\"{", "\"Result\":{").Replace("}\"}", "}}").Replace("}\"", "}").Replace("\"{", "{");
            return v;
        }

        #endregion

        #region  将 List<T> to  List<KeyValuePair<long, string>>

        public static List<KeyValuePair<long, string>> ToListKeyValuePair<T>(this List<T> value) where T : class
        {
            var result = new List<KeyValuePair<long, string>>();
            Type type = value.GetType();
            var count = type.GetProperty("Count").GetValue(value, null).Convert(0);
            for (int i = 0; i < count; i++)
            {
                var itemValue = type.GetProperty("Item").GetValue(value, new object[] { i });
                long autoNo = long.Parse(itemValue.GetType().GetProperty("AutoNo").GetValue(itemValue, null).Convert("0"));
                result.Add(new KeyValuePair<long, string>(autoNo, itemValue.ToJson()));
            }
            return result;
        }

        public static List<KeyValuePair<long, string>> ToListKeyValuePair(this object value)
        {
            var result = new List<KeyValuePair<long, string>>();
            Type type = value.GetType();
            var count = type.GetProperty("Count").GetValue(value, null).Convert(0);
            for (int i = 0; i < count; i++)
            {
                var itemValue = type.GetProperty("Item").GetValue(value, new object[] { i });
                long autoNo = long.Parse(itemValue.GetType().GetProperty("AutoNo").GetValue(itemValue, null).Convert("0"));
                result.Add(new KeyValuePair<long, string>(autoNo, itemValue.ToJson()));
            }
            return result;
        }
        public static List<KeyValuePair<long, string>> ToListKeyValuePairId(this object value)
        {
            var result = new List<KeyValuePair<long, string>>();
            Type type = value.GetType();
            var count = type.GetProperty("Count").GetValue(value, null).Convert(0);
            for (int i = 0; i < count; i++)
            {
                var itemValue = type.GetProperty("Item").GetValue(value, new object[] { i });
                long autoNo = long.Parse(itemValue.GetType().GetProperty("Id").GetValue(itemValue, null).Convert("0"));
                result.Add(new KeyValuePair<long, string>(autoNo, itemValue.ToJson()));
            }
            return result;
        }
        public static List<KeyValuePair<long, string>> ToListKeyValuePairScript(this object value)
        {
            var result = new List<KeyValuePair<long, string>>();
            Type type = value.GetType();
            var count = type.GetProperty("Count").GetValue(value, null).Convert(0);
            for (int i = 0; i < count; i++)
            {
                var itemValue = type.GetProperty("Item").GetValue(value, new object[] { i });
                long autoNo = long.Parse(itemValue.GetType().GetProperty("AutoNo").GetValue(itemValue, null).Convert("0"));
                ;
                result.Add(new KeyValuePair<long, string>(autoNo, itemValue.GetType().GetProperty("content").GetValue(itemValue, null).Convert("")));
            }
            return result;
        }
        #endregion

        public static void Get()
        {
            //动态创建的类类型
            Type classType = DynamicCreateType().AsType();
            //调用有参数的构造函数
            Type[] ciParamsTypes = new Type[] { typeof(string) };
            object[] ciParamsValues = new object[] { "Hello World" };
            ConstructorInfo ci = classType.GetConstructor(ciParamsTypes);
            object Vector = ci.Invoke(ciParamsValues);
            //调用方法
            object[] methedParams = new object[] { };


        }

        public static TypeInfo DynamicCreateType()

        {

            //动态创建程序集

            AssemblyName DemoName = new AssemblyName("DynamicAssembly");

            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(DemoName, AssemblyBuilderAccess.RunAndCollect);

            //动态创建模块

            ModuleBuilder mb = dynamicAssembly.DefineDynamicModule(DemoName.Name);

            //动态创建类MyClass

            TypeBuilder tb = mb.DefineType("MyClass", TypeAttributes.Public);

            //动态创建字段

            FieldBuilder fb = tb.DefineField("myField", typeof(System.String), FieldAttributes.Public);

            //动态创建构造函数

            Type[] clorType = new Type[] { typeof(System.String) };

            ConstructorBuilder cb1 = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, clorType);

            //生成指令

            ILGenerator ilg = cb1.GetILGenerator();//生成 Microsoft 中间语言 (MSIL) 指令

            ilg.Emit(OpCodes.Ldarg_0);

            ilg.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

            ilg.Emit(OpCodes.Ldarg_0);

            ilg.Emit(OpCodes.Ldarg_1);

            ilg.Emit(OpCodes.Stfld, fb);

            ilg.Emit(OpCodes.Ret);

            //动态创建属性

            PropertyBuilder pb = tb.DefineProperty("MyProperty", PropertyAttributes.HasDefault, typeof(string), null);

            //动态创建方法

            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName;

            MethodBuilder myMethod = tb.DefineMethod("get_Field", getSetAttr, typeof(string), Type.EmptyTypes);

            //生成指令

            ILGenerator numberGetIL = myMethod.GetILGenerator();

            numberGetIL.Emit(OpCodes.Ldarg_0);

            numberGetIL.Emit(OpCodes.Ldfld, fb);

            numberGetIL.Emit(OpCodes.Ret);

            //使用动态类创建类型

            TypeInfo classType = tb.CreateTypeInfo();



            return classType;

        }

        #region object或泛型

        /// <summary>
        /// 判断结构是否为默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        /// <summary>
        /// 判断是否是空数据(null或DBNull)
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool IsNothing(this object value)
        {
            return value == null;
        }

        /// <summary>
        /// 比较两个值
        /// </summary>
        /// <param name="value1">待比较值</param>
        /// <param name="value2">比较值</param>
        /// <returns></returns>
        public static bool CompareTo(this object value1, object value2)
        {
            bool isSystemType1 = Type.GetTypeCode(value1.GetType()) != TypeCode.Object;
            bool isSystemType2 = Type.GetTypeCode(value2.GetType()) != TypeCode.Object;

            bool success = false;
            if (isSystemType1)
            {
                //如果value1是基础类型.value2不是基础类型.但已实现IConvertible接口.则将value2转为value1类型
                if (!isSystemType2 && value2 is IConvertible)
                {
                    value2 = ((IConvertible)value2).ToType(value1.GetType(), null);
                }
                else if (value1.GetType() != value2.GetType() && value2 is IConvertible)
                {
                    value2 = ((IConvertible)value2).ToType(value1.GetType(), null);
                }
            }
            else if (isSystemType2)
            {
                //如果value2是基础类型.value1不是基础类型.但已实现IConvertible接口.则将value1转为value2类型
                if (!isSystemType1 && value1 is IConvertible)
                {
                    value1 = ((IConvertible)value1).ToType(value2.GetType(), null);
                }
                else if (value1.GetType() != value2.GetType() && value1 is IConvertible)
                {
                    value1 = ((IConvertible)value1).ToType(value2.GetType(), null);
                }
            }

            int result = 1;

            if (value1 is IComparable)
            {
                success = true;
                result = ((IComparable)value1).CompareTo(value2);
            }
            else if (value2 is IComparable)
            {
                success = true;
                //value2 与 value1相比较.所以结果为相反
                result = 0 - ((IComparable)value2).CompareTo(value1);
            }
            if (success && result == 0)
                return true;

            return false;
        }

        /// <summary>
        /// 发送方法或属性请求，确认方法或属性是否可用。
        /// 确认反射类中是否存在需要的方法或属性。
        /// </summary>
        /// <param name="value">反射类</param>
        /// <param name="member">反射方法或属性名</param>
        /// <param name="ensureNoParameters">确保没有参数</param>
        /// <returns>反射方法中存在对应的方法或属性</returns>
        public static bool Reflect_Respond(this object value, string member, bool ensureNoParameters = true)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type type = value.GetType();

            MethodInfo methodInfo = type.GetMethod(member);
            if (methodInfo != null && (!ensureNoParameters || !methodInfo.GetParameters().Any()))
                return true;

            PropertyInfo propertyInfo = type.GetProperty(member);
            if (propertyInfo != null && propertyInfo.CanRead)
                return true;

            return false;
        }

        /// <summary>
        /// 调用反射方法或属性并读取返回信息
        /// </summary>
        /// <param name="value">反射类</param>
        /// <param name="member">反射方法或属性名</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>属性值或方法返回值</returns>
        public static object Reflect_Call(this object value, string member, object[] parameters = null)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Type type = value.GetType();

            MethodInfo methodInfo = type.GetMethod(member);
            if (methodInfo != null)
                return methodInfo.Invoke(value, parameters);

            PropertyInfo propertyInfo = type.GetProperty(member);
            if (propertyInfo != null)
                return propertyInfo.GetValue(value, null);

            return null;
        }

        /// <summary>
        /// 反射获取对象属性值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象值</param>
        /// <param name="propertyname">属性名称</param>
        /// <returns>属性值</returns>
        public static object PropertyVal<T>(this T t, string propertyname) where T : class
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return null;

            return property.GetValue(t, null);
        }

        /// <summary>
        /// 反射设置对象属性值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t">对象值</param>
        /// <param name="propertyname">属性名称</param>
        /// <param name="propertyvalue">属性值</param>
        public static void PropertyVal<T>(this T t, string propertyname, object propertyvalue) where T : class
        {
            Type type = typeof(T);

            PropertyInfo property = type.GetProperty(propertyname);

            if (property == null) return;

            property.SetValue(t, propertyvalue, null);
        }

        /// <summary>
        /// 反射创建实体类
        /// </summary>
        public static T Create<T>(this Type t) where T : class, new()
        {
            return (T)Activator.CreateInstance(t);
        }

        /// <summary>
        /// 将对象类型转化成对应类型对象
        /// </summary>
        /// <param name="value">对象值</param>
        /// <param name="type">转化目标类型</param>
        /// <returns></returns>
        public static object ChangeTo(this object value, Type _type)
        {
            if (value.IsNothing())
            {
                switch (Type.GetTypeCode(_type))
                {
                    case TypeCode.Boolean: return false;
                    case TypeCode.Byte: return new Byte();
                    case TypeCode.Char: return new char();

                    case TypeCode.DateTime: return DateTime.Now;
                    case TypeCode.Decimal: return 0;
                    case TypeCode.Double: return 0;
                    case TypeCode.Empty: return "";
                    case TypeCode.Int16: return 0;
                    case TypeCode.Int32: return 0;
                    case TypeCode.Int64: return 0;
                    case TypeCode.Object: return new object();
                    case TypeCode.SByte: return new SByte();
                    case TypeCode.Single: return new Single();
                    case TypeCode.String: return "";
                    case TypeCode.UInt16: return 0;
                    case TypeCode.UInt32: return 0;
                    case TypeCode.UInt64: return 0;
                    default: return "";
                }
            }

            if (_type.Name.ToLower() == "enum")
            {
                if (value is int)
                    return Enum.ToObject(_type, value);
                else
                    return Enum.Parse(_type, value.ToString());
            }
            else if (Type.GetTypeCode(_type) == TypeCode.Boolean)
            {
                int v;
                if (Int32.TryParse(value.ToString(), out v))
                    value = v;
            }
            else if (Type.GetTypeCode(_type) == TypeCode.DateTime
                && value.ToString().Length == 8)
            {
                value = DateTime.ParseExact(value.ToString(), "yyyyMMdd",
                    new System.Globalization.CultureInfo("Zh-cn"));
            }

            return System.Convert.ChangeType(value, _type);
        }

        /// <summary>
        /// 强制数据类型转换成指定格式
        /// </summary>
        /// <typeparam name="T">指定数据类型</typeparam>
        /// <param name="value">需转换值</param>
        public static T Convert<T>(this object value)
        {
            return value.Convert<T>(default(T));
        }

        /// <summary>
        /// 强制数据类型转换成指定格式
        /// </summary>
        /// <typeparam name="T">指定数据类型</typeparam>
        /// <param name="value">需转换值</param>
        public static T Convert<T>(this object value, T defaultValue)
        {
            if (value.IsNothing())
                return defaultValue;

            if (value is string && value == null)
                return defaultValue;

            if (value is bool && Type.GetTypeCode(typeof(T)) == TypeCode.Int32)
            {
                value = (bool)value ? 1 : 0;
            }
            else if (typeof(T).Name.ToLower() == "enum")
            {
                if (value is int)
                    return (T)Enum.ToObject(typeof(T), value);
                else
                    return (T)Enum.Parse(typeof(T), value.ToString());
            }
            else if (Type.GetTypeCode(typeof(T)) == TypeCode.Boolean)
            {
                if (value.ToString().Length == 1)
                {
                    int v;
                    if (Int32.TryParse(value.ToString(), out v))
                        value = v;
                }
            }
            else if (Type.GetTypeCode(typeof(T)) == TypeCode.DateTime
                && value.ToString().Length == 8)
            {
                value = DateTime.ParseExact(value.ToString(), "yyyyMMdd",
                    new System.Globalization.CultureInfo("Zh-cn"));
            }
            else if (value is string && Type.GetTypeCode(typeof(T)) == TypeCode.Int32)
            {
                string v = value.ToString();
                if (v.Equals("false", StringComparison.OrdinalIgnoreCase) || v.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    value = v.Equals("false", StringComparison.OrdinalIgnoreCase) ? 0 : 1;
                }
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
        }






        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 转成实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this string json) where T : class
        {

            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 转成实例
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ToEntity(this string json, Type type)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            return serializer.Deserialize(new JsonTextReader(sr), type);
        }


        /// <summary>
        /// 获取所有属性值 定义于2017/8/3 下午 3点 32 为解决关于Reflection和MySql.Data.MySqlClient 都拥有GetProperties方法的冲突
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo[] Properties(this Type type)
        {
            return type.GetProperties();
        }

        /// <summary>
        /// 旧值合并新值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static T MergeOldEntity<T>(this T v, T oldValue) where T : class
        {
            var oldtype = oldValue.GetType();
            var oldties = oldtype.GetProperties();
            var newtype = v.GetType();
            var newties = newtype.GetProperties();

            foreach (var np in newties)
            {
                var npv = np.GetValue(v, null);
                var typeee = np.GetType().Name;
                if (np.PropertyType == typeof(int) || np.PropertyType == typeof(long))
                {
                    if (long.Parse(npv.ToString()) == 0)
                    {
                        continue;
                    }
                }
                if (npv != null && np.Name != "Factory")
                {
                    oldtype.GetProperty(np.Name).SetValue(oldValue, npv);
                }
            }

            return oldValue;

        }

        #endregion.
    }
}
