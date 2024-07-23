using System;

namespace IpisCentralDisplayController.models
{
    public enum RegionalLanguage
    {
        ENGLISH, 
        HINDI,
        ASSAMESE,        
        BANGLA,
        BODO,
        DOGRI,
        GUJARATI,
        KANNADA,
        KASHMIRI,
        KONKANI,
        MALAYALAM,
        MARATHI,
        MANIPURI,
        NEPALI,
        ODIA,
        PUNJABI,
        SANSKRIT,
        SANTHALI,
        SINDHI,
        TAMIL,
        TELUGU,
        URDU
    }

    public static class RegionalLanguageEnum
    {
        public static Array Values
        {
            get { return Enum.GetValues(typeof(RegionalLanguage)); }
        }
    }
}
