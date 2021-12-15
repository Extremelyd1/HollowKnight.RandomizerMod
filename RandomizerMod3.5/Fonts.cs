using UnityEngine;

namespace RandomizerMod
{
    public static class Fonts
    {
        private static Font Perpetua;

        private static void LoadFonts()
        {
            foreach (var font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font.name.Contains("perpetua") || font.name.Contains("Perpetua"))
                {
                    Perpetua = font;
                }
            }
        }

        public static Font Get(string name)
        {
            if (Perpetua == null)
            {
                LoadFonts();
            }

            return Perpetua;
        }
    }
}
