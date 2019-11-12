using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace WCMS.FrameWork.Data.Common
{
    /// <summary>
    /// 확장 클래스
    /// </summary>
    public static class DBExtensions
    {
        #region ToList
        /// <summary>
        ///  Converts a DataReader to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="reader">DataReader</param>
        /// <returns>List with generic objects</returns>
        public static List<T> ToList<T>(this DbDataReader reader) where T : class, new()
        {
            var list = new List<T>();

            try
            {
                while (reader.Read())
                {
                    T item = new T();
                    foreach (var propertyInfo in typeof(T).GetProperties())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(propertyInfo.Name)))
                        {
                            Type convertTo = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                            propertyInfo.SetValue(item, Convert.ChangeType(reader[propertyInfo.Name], convertTo), null);
                        }
                    }
                    list.Add(item);
                }
            }
            catch
            {
                return null;
            }
            return list;
        }
        
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            var list = new List<T>();

            try
            {
                foreach (var row in table.AsEnumerable())
                {
                    T item = new T();
                    foreach (var propertyInfo in typeof(T).GetProperties())
                    {
                        if (!row.IsNull(propertyInfo.Name))
                        {
                            Type convertTo = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                            propertyInfo.SetValue(item, Convert.ChangeType(row[propertyInfo.Name], convertTo), null);
                        }
                    }
                    list.Add(item);
                }
            }
            catch
            {
                return null;
            }
            return list;
        }
        #endregion

        #region ToDataTable
        /// <summary>
        /// Type 확장 함수
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="memberTypes"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this Type type, string typeName = null, MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property)
        {
            return ToDataTable(null, type, typeName, memberTypes, null);
        }
        /// <summary>
        /// Converts generic enumerator to a DataTable
        /// 사용법: 
        /// using (var DB = new DapperHelper("LogDBConnection"))
        /// {
        ///     var result = DB.Query<Address>("pFront_Addr_Search", new { sDong = "회현" }, CommandType.StoredProcedure).ToDataTable<Address>();
        /// }
        /// </summary>
        /// <typeparam name="T">Gerneric object</typeparam>
        /// <param name="instances">enumerator</param>
        /// <param name="typeName">TableName</param>
        /// <param name="memberTypes">MemberTypes 열거형</param>
        /// <param name="table">DataTable</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> instances, string typeName = null, MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property, DataTable table = null)
        {
            return ToDataTable(instances, typeof(T), typeName, memberTypes, table);
        }
        /// <summary>
        /// Converts generic enumerator to a DataTable
        /// </summary>
        /// <param name="instances">enumerator</param>
        /// <param name="type">The type of objects to enumerate</param>
        /// <param name="typeName">TableName</param>
        /// <param name="memberTypes">MemberTypes 열거형</param>
        /// <param name="table">DataTable</param>
        /// <returns>DataTable</returns>
        private static DataTable ToDataTable(IEnumerable instances, Type type, string typeName, MemberTypes memberTypes, DataTable table)
        {
            bool isField = ((memberTypes & MemberTypes.Field) == MemberTypes.Field);
            bool isProperty = ((memberTypes & MemberTypes.Property) == MemberTypes.Property);

            var columns =
                type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => isField)
                    .Select(f => new
                    {
                        ColumnName = f.Name,
                        ColumnType = f.FieldType,
                        IsField = true,
                        MemberInfo = (MemberInfo)f
                    })
                    .Union(
                        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => isProperty)
                            .Where(p => p.CanRead)
                            .Where(p => p.GetGetMethod(true).IsPublic)
                            .Where(p => p.GetIndexParameters().Length == 0)
                            .Select(p => new
                            {
                                ColumnName = p.Name,
                                ColumnType = p.PropertyType,
                                IsField = false,
                                MemberInfo = (MemberInfo)p
                            })
                    )
                    .OrderBy(c => c.MemberInfo.MetadataToken);

            if (table == null)
            {
                table = new DataTable();

                table.Columns.AddRange(
                    columns.Select(c => new DataColumn(
                        c.ColumnName,
                        (c.ColumnType.IsGenericType && c.ColumnType.GetGenericTypeDefinition() == typeof(Nullable<>) ? c.ColumnType.GetGenericArguments()[0] : c.ColumnType)
                    )).ToArray()
                );
            }

            if (instances != null)
            {
                table.BeginLoadData();

                try
                {
                    foreach (var instance in instances)
                    {
                        if (instance != null)
                        {
                            DataRow row = table.NewRow();

                            foreach (var column in columns)
                                row[column.ColumnName] = (column.IsField ? ((FieldInfo)column.MemberInfo).GetValue(instance) : ((PropertyInfo)column.MemberInfo).GetValue(instance, null)) ?? DBNull.Value;

                            table.Rows.Add(row);
                        }
                    }
                }
                finally
                {
                    table.EndLoadData();
                }
            }

            table.TableName = typeName;

            return table;
        }

        #endregion
    }
}
