﻿namespace LambdicSql.BuilderServices.CodeParts
{
    /// <summary>
    /// Customizer.
    /// </summary>
    public interface ICodeCustomizer
    {
        /// <summary>
        /// Coustom.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <returns>Result.</returns>
        ICode Custom(ICode src);
    }
}
