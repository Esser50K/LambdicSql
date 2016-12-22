﻿namespace LambdicSql.SqlBase.TextParts
{
    //TODO \tをテーブル化すると早くなる
    //抽象メソッドをなくしても早くなる
    //string.joinもEX
    //Linqをなくしても早くなる

    //SqlPartsかな
    //でネームスペースはSqlBuilder

    /// <summary>
    /// Text.
    /// </summary>
    public abstract class SqlText
    {
        /// <summary>
        /// Is single line.
        /// </summary>
        public abstract bool IsSingleLine { get; }

        /// <summary>
        /// Is empty.
        /// </summary>
        public abstract bool IsEmpty { get; }
        
        /// <summary>
        /// To string.
        /// </summary>
        /// <param name="isTopLevel">Is top level.</param>
        /// <param name="indent">Indent.</param>
        /// <param name="context">Context.</param>
        /// <returns>Text.</returns>
        public abstract string ToString(bool isTopLevel, int indent, SqlConvertingContext context);

        /// <summary>
        /// Concat to front and back.
        /// </summary>
        /// <param name="front">Front.</param>
        /// <param name="back">Back.</param>
        /// <returns>Text.</returns>
        public abstract SqlText ConcatAround(string front, string back);

        /// <summary>
        /// Concat to front.
        /// </summary>
        /// <param name="front">Front.</param>
        /// <returns>Text.</returns>
        public abstract SqlText ConcatToFront(string front);

        /// <summary>
        /// Concat to back.
        /// </summary>
        /// <param name="back"></param>
        /// <returns></returns>
        public abstract SqlText ConcatToBack(string back);

        /// <summary>
        /// Customize.
        /// </summary>
        /// <param name="customizer">Customizer.</param>
        /// <returns>Customized SqlText.</returns>
        public abstract SqlText Customize(ISqlTextCustomizer customizer);

        /// <summary>
        /// Convert string to IText.
        /// </summary>
        /// <param name="text">string.</param>
        public static implicit operator SqlText(string text) => new SingleText(text);
    }
}
