using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ScreemDisplay;
using PCD_ColorFull;
using PCD_INOUT_INFO;


namespace PCD_EVENT_DATA
{
    public enum TypePost
    {
        _LEFT = 0,
        _UP_LEFT = 1,
        _UP_CENTER = 2,
        _DOWN_LEFT = 3,
        _DOWN_CENTER = 4
    }
    public enum TypeDataIN
    {
        _none = -1,
        _LETTER = 0,
        _SHIFT_LETTER = 1,
        _NUMERIC_FILE = 2,
        _SHIFT_NUMERIC_FILE = 3,
        _NUMERIC_PAD = 4,
        _CARACTER_SPECIAL = 5,
        _SHIFT_CARACTER_SPECIAL = 6,
        _SPACE = 7
    }
    public enum TypeStateIO
    {
        _INACTIVATED = 0,
        _SEMIACTIVATED = 1,
        _ACTIVATED = 2,
        _ERROR_DATA = 3 
    }
    public enum TypeINFO
    {
        _none = 0,
        _NORMAL = 1,
        _PASSWORD = 2
    }
    public abstract class IOdata
    {
        protected TypeStateIO _StateEvent;

        public IOdata() 
        {
            this._StateEvent = TypeStateIO._INACTIVATED;
        }
        public void SetInactivated()
        {
            this._StateEvent = TypeStateIO._INACTIVATED;
        }
        public void SetSemiInactivated()
        {
            this._StateEvent = TypeStateIO._SEMIACTIVATED;
        }
        public void SetActivated()
        {
            this._StateEvent = TypeStateIO._ACTIVATED;
        }
        public void SetErrorData()
        {
            this._StateEvent = TypeStateIO._ERROR_DATA;
        }
        public bool CompTypeDataIN(TypeDataIN verif, int tipekey)
        {
            int bit_cond = ((int)verif);
            bool estado = false;
            if (((tipekey >> bit_cond) & 1) == 1) { estado = true; }
            return estado;
        }
        public void ConfiguradorKeyInfo(ref IN keydata, int typekey)
        {
            keydata.SetCondIN(INCond._ENTER);
            keydata.SetCondIN(INCond._BACKSPACE);

            if (CompTypeDataIN(TypeDataIN._LETTER, typekey))
            {
                keydata.SetCondIN(INCond._LETTER);
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_LETTER, typekey))
            {
                keydata.SetCondIN(INCond._SHIFT);
                keydata.SetCondIN(INCond._LETTER);
            }
            if (CompTypeDataIN(TypeDataIN._NUMERIC_FILE, typekey))
            {
                keydata.SetCondIN(INCond._NUMERS);
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_NUMERIC_FILE, typekey))
            {
                keydata.SetCondIN(INCond._SHIFT);
                keydata.SetCondIN(INCond._NUMERS);
            }
            if (CompTypeDataIN(TypeDataIN._NUMERIC_PAD, typekey))
            {
                keydata.SetCondIN(INCond._NUMPADS);
            }
            if (CompTypeDataIN(TypeDataIN._CARACTER_SPECIAL, typekey))
            {
                keydata.SetCondIN(INCond._OEMSKEY);
                keydata.SetCondIN(INCond._OEMARITM);
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_CARACTER_SPECIAL, typekey))
            {
                keydata.SetCondIN(INCond._SHIFT);
                keydata.SetCondIN(INCond._OEMSKEY);
                keydata.SetCondIN(INCond._OEMARITM);
            }
            if (CompTypeDataIN(TypeDataIN._SPACE, typekey))
            {
                keydata.SetCondIN(INCond._SPACEBAR);
            }
        }
        public bool VerifIngresoTecla(ref string tecla, int typekey)
        {
            bool script = false;
            int val = 0;
            if (CompTypeDataIN(TypeDataIN._LETTER, typekey))
            {
                if (tecla.Length == 1)
                {
                    val = (int)((char)tecla[0]);
                    if (((val >= 65) && (val <= 90)) || (val == 209))
                    {
                        if ((val >= 65) && (val <= 90)) { tecla = ((char)(val + 32)).ToString(); }
                        if (val == 209) { tecla = ((char)(241)).ToString(); }
                        script = true;
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_LETTER, typekey))
            {
                if (tecla.Length == 9)
                {
                    if ((tecla.Substring(0, 8)).Equals("SHIFT + "))
                    {
                        string h = tecla.Remove(0, 8);
                        val = (int)((char)h[0]);
                        if (((val >= 65) && (val <= 90)) || (val == 209))
                        {
                            tecla = tecla.Remove(0, 8);
                            script = true;
                        }
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._NUMERIC_FILE, typekey))
            {
                if (tecla.Length == 1)
                {
                    val = (int)((char)tecla[0]);
                    if ((val >= 48) && (val <= 57))
                    {
                        script = true;
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_NUMERIC_FILE, typekey))
            {
                if (tecla.Length == 9)
                {
                    if ((tecla.Substring(0, 8)).Equals("SHIFT + "))
                    {
                        string j = tecla.Remove(0, 8);
                        val = (int)((char)j[0]);
                        if ((val >= 48) && (val <= 57))
                        {
                            switch (val)
                            {
                                case (48): tecla = "="; break;
                                case (49): tecla = "!"; break;
                                case (50): tecla = "@"; break;
                                case (51): tecla = "#"; break;
                                case (52): tecla = "$"; break;
                                case (53): tecla = "%"; break;
                                case (54): tecla = "&"; break;
                                case (55): tecla = "/"; break;
                                case (56): tecla = "("; break;
                                case (57): tecla = ")"; break;
                            }
                            script = true;
                        }
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._NUMERIC_PAD, typekey))
            {
                if (tecla.Length == 8)
                {
                    if ((tecla.Substring(0, 7)).Equals("NUMPAD_"))
                    {
                        tecla = tecla.Remove(0, 7);
                        val = (int)((char)tecla[0]);
                        if ((val >= 48) && (val <= 57))
                        {
                            script = true;
                        }
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._CARACTER_SPECIAL, typekey))
            {
                if ((tecla.Length == 4) || (tecla.Length == 6))
                {
                    if ((tecla.Substring(0, 3)).Equals("Oem"))
                    {
                        switch (tecla)
                        {
                            case ("Oem1"): tecla = "´"; break;
                            case ("Oem2"): tecla = "}"; break;
                            case ("Oem4"): tecla = "'"; break;
                            case ("Oem5"): tecla = "|"; break;
                            case ("Oem6"): tecla = "¿"; break;
                            case ("Oem7"): tecla = "{"; break;
                            case ("Oem102"): tecla = "<"; break;
                            case ("Oem+"): tecla = "+"; break;
                            case ("Oem-"): tecla = "-"; break;
                            case ("OemP"): tecla = "."; break;
                            case ("OemC"): tecla = ","; break;
                        }
                        script = true;
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._SHIFT_CARACTER_SPECIAL, typekey))
            {
                if ((tecla.Length == 12) || (tecla.Length == 14))
                {
                    if ((tecla.Substring(0, 11)).Equals("SHIFT + Oem"))
                    {
                        tecla = tecla.Remove(0, 8);
                        switch (tecla)
                        {
                            case ("Oem1"): tecla = "¨"; break;
                            case ("Oem2"): tecla = "]"; break;
                            case ("Oem4"): tecla = "?"; break;
                            case ("Oem5"): tecla = "°"; break;
                            case ("Oem6"): tecla = "¡"; break;
                            case ("Oem7"): tecla = "["; break;
                            case ("Oem102"): tecla = ">"; break;
                            case ("Oem+"): tecla = "*"; break;
                            case ("Oem-"): tecla = "_"; break;
                            case ("OemP"): tecla = ":"; break;
                            case ("OemC"): tecla = ";"; break;
                        }
                        script = true;
                    }
                }
            }
            if (CompTypeDataIN(TypeDataIN._SPACE, typekey))
            {
                if (tecla.Equals("SPACEBAR"))
                {
                    tecla = " ";
                    script = true;
                }
            }

            return (script);
        }
        public static void Checkeador(color bf, color ff, color bb, int x, int y)
        {
            OUT.PrintLine("╔═╗", ff, bf, x, y);
            OUT.PrintLine("║ ║", ff, bf, x, y + 1);
            OUT.PrintLine("╚═╝", ff, bf, x, y + 2);
            OUT.PrintLine(" ", ff, bb, x + 1, y + 1);
        }
        public static void Selector(color bf, color ff, string select, int grosor, int x, int y)
        {
            string fila = "";
            for (int i = 0; i < grosor + 2; i++)
            {
                fila = String.Concat(fila, " ");
            }
            select = String.Concat(" ", select);
            int kot = ((grosor + 2) - (select.Length));
            for (int i = 0; i < kot; i++)
            {
                select = String.Concat(select, " ");
            }
            OUT.PrintLine(fila, ff, bf, x, y);
            OUT.PrintLine(select, ff, bf, x, y + 1);
            OUT.PrintLine(fila, ff, bf, x, y + 2);
        }
        public static void SelectorMedio(color bf, color ff, string select, int grosor, int x, int y)
        {
            string fila = "", cond = "";
            for (int i = 0; i < (grosor + 2); i++)
            {
                fila = String.Concat(fila, " ");
            }
            for(int i = 0; i < (((grosor + 2) - select.Length) / 2); i++)
            {
                cond = String.Concat(cond, " ");
            }
            cond = String.Concat(cond, select);
            
            for (int i = cond.Length; i < grosor + 2; i++)
            {
                cond = String.Concat(cond, " ");
            }
            OUT.PrintLine(fila, ff, bf, x, y);
            OUT.PrintLine(cond, ff, bf, x, y + 1);
            OUT.PrintLine(fila, ff, bf, x, y + 2);
        }
        public object ConvertMes(object dat)
        {
            string[] scriptmes = {  "    Enero     ", "   Febrero    ", "    Marzo     ",
                                    "    Abril     ", "     Mayo     ", "    Junio     ",
                                    "    Julio     ", "    Agosto    ", "  Septiembre  ",
                                    "   Octubre    ", "  Noviembre   ", "  Diciembre   " };
            object valor = new object();

            if (dat is Int32) { valor = scriptmes[Convert.ToInt32(dat) - 1]; }
            if (dat is String)
            {
                int k = 0;
                while (!(scriptmes[k].Equals(dat.ToString())))
                {
                    k++;
                }
                valor = k + 1;
            }
            return (valor);
        }
        public int TopeDia(int mes, int anio)
        {
            int k = 31;  //1 3, 5, 7, 8, 10, 12

            if ((mes == 4) || (mes == 6) || (mes == 9) || (mes == 11)) { k = 30; }
            else
            {
                if (mes == 2)
                {
                    k = 28;
                    if (Bisiesto(anio)) { k = 29; }
                }
            }
            return (k);
        }
        public bool Bisiesto(int anio)
        {
            bool valor = false;
            if ((((anio % 4) == 0) && ((anio % 100) != 0)) || ((anio % 400) == 0)) { valor = true; }
            return (valor);
        }
    }
}
