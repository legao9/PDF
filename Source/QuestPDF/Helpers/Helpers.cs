﻿using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Helpers
{
    internal static class Helpers
    {
        internal static byte[] LoadEmbeddedResource(string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using var reader = new BinaryReader(stream);
            
            return reader.ReadBytes((int) stream.Length);
        }
        
        private static PropertyInfo? ToPropertyInfo<T, TValue>(this Expression<Func<T, TValue>> selector)
        {
            return (selector.Body as MemberExpression)?.Member as PropertyInfo;
        }
        
        internal static string? GetPropertyName<T, TValue>(this Expression<Func<T, TValue>> selector) where TValue : class
        {
            return selector.ToPropertyInfo()?.Name;
        }
        
        internal static TValue? GetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> selector) where TValue : class
        {
            return selector.ToPropertyInfo()?.GetValue(target) as TValue;
        }
        
        internal static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> selector, TValue value)
        {
            var property = selector.ToPropertyInfo() ?? throw new Exception("Expected property with getter and setter.");
            property?.SetValue(target, value);
        }

        internal static string PrettifyName(this string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled);
        }

        internal static void VisitChildren(this Element? element, Action<Element?> handler)
        {
            if (element == null)
                return;
            
            foreach (var child in element.GetChildren())
                VisitChildren(child, handler);

            handler(element);
        }

        internal static bool IsNegative(this Size size)
        {
            return size.Width < 0f || size.Height < 0f;
        }
        
        internal static int ToQualityValue(this ImageCompressionQuality quality)
        {
            return quality switch
            {
                ImageCompressionQuality.Best => 100,
                ImageCompressionQuality.VeryHigh => 90,
                ImageCompressionQuality.High => 80,
                ImageCompressionQuality.Medium => 60,
                ImageCompressionQuality.Low => 40,
                ImageCompressionQuality.VeryLow => 20,
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }

        internal static SKFilterQuality ToFilterQuality(this ImageScalingQuality quality)
        {
            return quality switch
            {
                ImageScalingQuality.Low => SKFilterQuality.None,
                ImageScalingQuality.Medium => SKFilterQuality.Low,
                ImageScalingQuality.High => SKFilterQuality.Medium,
                ImageScalingQuality.Best => SKFilterQuality.High,
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, null)
            };
        }
    }
}