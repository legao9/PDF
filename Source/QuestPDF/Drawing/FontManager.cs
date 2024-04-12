﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    /// <summary>
    /// <para>By default, the library searches all fonts available in the runtime environment.</para>
    /// <para>This may work well on the development environment but may fail in the cloud where fonts are usually not installed.</para>
    /// <para>It is safest deploy font files along with the application and then register them using this class.</para>
    /// </summary>
    public static class FontManager
    {
        private static SkTypefaceProvider TypefaceProvider { get; } = new();
        
        private static SkFontCollection LocalFontCollection { get; } = SkFontCollection.Create(TypefaceProvider, SkFontManager.Local);
        private static SkFontCollection GlobalFontCollection { get; } = SkFontCollection.Create(TypefaceProvider, SkFontManager.Global);
        
        internal static SkFontCollection CurrentFontCollection => Settings.UseEnvironmentFonts ? GlobalFontCollection : LocalFontCollection;
        internal static SkFontManager CurrentFontManager => Settings.UseEnvironmentFonts ? SkFontManager.Global : SkFontManager.Local;

        static FontManager()
        {
            NativeDependencyCompatibilityChecker.Test();
        }
        
        [Obsolete("Since version 2022.8 this method has been renamed. Please use the RegisterFontWithCustomName method.")]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            RegisterFontWithCustomName(fontName, stream);
        }
        
        /// <summary>
        /// Registers a TrueType font from a stream under the provided custom <paramref name="fontName"/>.
        /// Refer to this font by using the same name as a font family in the <see cref="TextStyle"/> API later on.
        /// <a href="https://www.questpdf.com/going-production/font-management.html">Learn more</a>
        /// </summary>
        public static void RegisterFontWithCustomName(string fontName, Stream stream)
        {
            using var fontData = SkData.FromStream(stream);
            TypefaceProvider.AddTypefaceFromData(fontData);
            TypefaceProvider.AddTypefaceFromData(fontData, fontName);
        }

        /// <summary>
        /// Registers a TrueType font from a stream. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/going-production/font-management.html">Learn more</a>
        /// </summary>
        public static void RegisterFont(Stream stream)
        {
            using var fontData = SkData.FromStream(stream);
            TypefaceProvider.AddTypefaceFromData(fontData);
        }
        
        /// <summary>
        /// Registers a TrueType font from an embedded resource. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/going-production/font-management.html">Learn more</a>
        /// </summary>
        /// <param name="pathName">Path to the embedded resource (the case-sensitive name of the manifest resource being requested).</param>
        public static void RegisterFontFromEmbeddedResource(string pathName)
        {
            using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(pathName);

            if (stream == null)
                throw new ArgumentException($"Cannot load font file from an embedded resource. Please make sure that the resource is available or the path is correct: {pathName}");
            
            RegisterFont(stream);
        }
    }
}