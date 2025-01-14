﻿using System;

namespace RandomizerMod.Extensions
{
    public static class StringExtensions
    {
        public static bool TryToEnum<T>(this string self, out T val) where T : Enum
        {
            try
            {
                val = (T)Enum.Parse(typeof(T), self);
                return true;
            }
            catch
            {
                val = default;
                return false;
            }
        }
    }
}
