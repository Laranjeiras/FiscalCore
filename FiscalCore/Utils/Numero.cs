using System;
using System.Globalization;

namespace FiscalCore.Utils
{
    public static class Numero
    {
        public static decimal Arredondar(this decimal valor, int casasDecimais)
        {
            var valorNovo = decimal.Round(valor, casasDecimais, MidpointRounding.AwayFromZero);
            var valorNovoStr = valorNovo.ToString("F" + casasDecimais, CultureInfo.CurrentCulture);
            return decimal.Parse(valorNovoStr);
        }

        public static decimal? Arredondar(this decimal? valor, int casasDecimais)
        {
            if (valor == null) return null;
            return Arredondar(valor.Value, casasDecimais);
        }

        #region Data

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTimeOffset data)
        {
            return data == DateTimeOffset.MinValue ? null : data.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTimeOffset? data)
        {
            if (data == null) return null;
            return data == DateTimeOffset.MinValue ? null : data.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTimeOffset data)
        {
            return data == DateTimeOffset.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:dd
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTimeOffset data)
        {
            return data == DateTimeOffset.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:dd
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTimeOffset? data)
        {
            return ParaDataHoraStringSemUtc(data.GetValueOrDefault());
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTimeOffset? data)
        {
            return ParaDataHoraStringUtc(data.GetValueOrDefault());
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD HH:mm:ss
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraString(this DateTimeOffset data)
        {
            return data.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataString(this DateTime? data)
        {
            if (data == null) return null;
            return data == DateTime.MinValue ? null : data.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:dd
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTime data)
        {
            return data == DateTime.MinValue ? null : data.ToString("yyyy-MM-ddTHH:mm:dd");
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:dd
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringSemUtc(this DateTime? data)
        {
            return ParaDataHoraStringSemUtc(data.GetValueOrDefault());
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DDThh:mm:ssTZD (UTC - Universal Coordinated Time)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraStringUtc(this DateTime? data)
        {
            return ParaDataHoraStringUtc(data.GetValueOrDefault());
        }

        /// <summary>
        ///     Retorna uma string no formato AAAA-MM-DD HH:mm:ss
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ParaDataHoraString(this DateTime data)
        {
            return data.ToString("yyyyMMddHHmmss");
        }

        public static string ParaHoraString(this TimeSpan hora)
        {
            return hora.ToString(@"hh\:mm\:ss");
        }

        #endregion

    }
}
