using System;
using System.Data;

namespace PROYEC_QUIMPAC.Utility
{
    public class HelperDAL
    {
        public static Int16? Int16(Object value)
        {
            if ((value == DBNull.Value) || (value == null))
            {
                return null;
            }
            else
            {
                return Convert.ToInt16(value);
            }
        }

        public static Int16? Int16(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                return Convert.ToInt16(value);
            }
        }

        public static Int32? NullableInt32(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        public static Int32? NullableInt32(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        public static Int32 Int32(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        public static Int32 Int32(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(value);
            }
        }

        public static decimal? NullableDecimal(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(value);
            }
        }

        public static decimal? NullableDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                if (IsNumber(value) == true)
                {
                    return Convert.ToDecimal(value);
                }
                else
                {
                    throw new ApplicationException("El Número ingresado es incorrecto.");
                }
            }
        }

        public static decimal Decimal(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(value);
            }
        }

        public static decimal Decimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            else
            {
                if (IsNumber(value) == true)
                {
                    return Convert.ToDecimal(value);
                }
                else
                {
                    throw new ApplicationException("El Número ingresado es incorrecto.");
                }
            }
        }

        public static string String(string value)
        {
            if (Convert.IsDBNull(value) || (string.IsNullOrEmpty(value)))
            {
                return null;
            }
            else
            {
                return Convert.ToString(value).Trim();
            }
        }

        public static string String(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)) == true)
                {
                    return null;
                }
                return Convert.ToString(value).Trim();
            }
        }

        public static string StringNoFormat(string value)
        {
            if (Convert.IsDBNull(value) || (string.IsNullOrEmpty(value)))
            {
                return null;
            }
            else
            {
                return Convert.ToString(value);
            }
        }

        public static string StringNoFormat(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return null;
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)) == true)
                {
                    return null;
                }
                return Convert.ToString(value);
            }
        }

        public static char Char(string value)
        {
            if (Convert.IsDBNull(value) || (string.IsNullOrEmpty(value)))
            {
                return Convert.ToChar(string.Empty);
            }
            else
            {
                return Convert.ToChar(value);
            }
        }

        public static char Char(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null))
            {
                return Convert.ToChar(string.Empty);
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(value)) == true)
                {
                    return Convert.ToChar(string.Empty);
                }
                return Convert.ToChar(value);
            }
        }

        public static DateTime? NullableDateTime(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null) || (value.ToString().Length == 0))
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(value);
            }
        }

        public static DateTime? NullableDateTime(string value, string format)
        {
            if (Convert.IsDBNull(value) || (string.IsNullOrEmpty(value)) || (value.Trim().Length == 0))
            {
                return null;
            }
            else
            {
                return System.DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static DateTime DateTime(Object value)
        {
            if (Convert.IsDBNull(value) || (value == null) || (value.ToString().Length == 0))
            {
                return new DateTime().Date;
            }
            else
            {
                return Convert.ToDateTime(value);
            }
        }

        public static DateTime DateTime(string value, string format)
        {
            if (Convert.IsDBNull(value) || (string.IsNullOrEmpty(value)) || (value.Trim().Length == 0))
            {
                return new DateTime().Date;
            }
            else
            {
                return System.DateTime.ParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).ToUpper() == columnName.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNumber(Object oCadena)
        {
            try
            {
                Double.Parse(oCadena.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
