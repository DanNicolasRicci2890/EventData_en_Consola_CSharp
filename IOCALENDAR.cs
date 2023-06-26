using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IOCALENDAR : IOdata, FuncIOData
    {
        private string _Titulo;
        private string _Anio;
        private string _Mes;
        private string _Dia;
        private color[] _BackCorral;
        private color[] _ForeCorral;
        private color[] _BackTitulo;
        private color[] _ForeTitulo;
        private color[] _BackSelect;
        private color[] _ForeSelect;
        private color[] _BackPointer;
        private color[] _ForePointer;
        private TypeLine _Linea;

        private int _PosX;
        private int _PosY;

        public IOCALENDAR(string titulo, color[] backCorral, color[] foreCorral, color[] backTitulo, color[] foreTitulo, color[] backSelect, color[] foreSelect, color[] backPointer, color[] forePointer, TypeLine lp, int posX, int posY)
        {
            this._Titulo = titulo;
            this._Anio = DateTime.Now.Year.ToString();
            this._Mes = "    Enero     ";
            this._Dia = "01";
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._BackTitulo = backTitulo;
            this._ForeTitulo = foreTitulo;
            this._BackSelect = backSelect;
            this._ForeSelect = foreSelect;
            this._BackPointer = backPointer;
            this._ForePointer = forePointer;
            this._Linea = lp;
            this._PosX = posX;
            this._PosY = posY;
        }

        public void Display(color back, color fore)
        {
            bool estado = true;
            int[] heinght;
            int condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
            color bcorral = this._BackCorral[condicion_color];
            color fcorral = this._ForeCorral[condicion_color];
            color btitulo = this._BackTitulo[condicion_color];
            color ftitulo = this._ForeTitulo[condicion_color];
            IN keydata = new IN();
            string tecla = "";
            int i = 0, pos = 0, d, m, a, k, g, tope_dia;
            int[,] calendario;
            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                keydata.SetCondIN(INCond._ARROWS);
                keydata.SetCondIN(INCond._TAB);
                keydata.SetCondIN(INCond._SHIFT);
                keydata.SetCondIN(INCond._ENTER);
                heinght = new int[3];
                heinght[0] = 3;
                heinght[1] = 3;
                heinght[2] = 30;
            }
            else
            {
                heinght = new int[2];
                heinght[0] = 3;
                heinght[1] = 4;
            }
            DRAW.TablaLine(this._Linea, bcorral, fcorral, new int[] { 37 }, heinght, this._PosX, this._PosY);
            SelectorMedio(btitulo, ftitulo, this._Titulo, 34, (this._PosX + 1), (this._PosY + 1));
            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                string kl = "";
                DRAW.TablaLine(TypeLine._SIMPLE, bcorral, fcorral, new int[] { 4, 4, 4, 4, 4, 4, 5 }, new int[] { 3, 3, 3, 3, 3, 3 , 4 }, this._PosX + 1, this._PosY + 9);
                for(int h = 0; h < 7; h++)
                {
                    switch(h)
                    {
                        case 0: kl = " DO "; break;
                        case 1: kl = " LU "; break;
                        case 2: kl = " MA "; break;
                        case 3: kl = " MI "; break;
                        case 4: kl = " JU "; break;
                        case 5: kl = " VI "; break;
                        case 6: kl = " SA "; break;
                    }
                    SelectorMedio(color.NEGRO, color.MAGENTA, kl, 2, this._PosX + 2 + (h * 5), this._PosY + 10);
                }                                
            }
            while (estado)
            {
                for (i = 0; i < 3; i++)
                {
                    condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());

                    if (this._StateEvent == TypeStateIO._ACTIVATED)
                    {
                        if (pos == i) { condicion_color++; }
                    }
                    color bselect = this._BackSelect[condicion_color];
                    color fselect = this._ForeSelect[condicion_color];

                    switch (i)
                    {
                        case 0: SelectorMedio(bselect, fselect, this._Dia, 4, this._PosX + 1, this._PosY + 5); break;
                        case 1: SelectorMedio(bselect, fselect, this._Mes, 14, this._PosX + 10, this._PosY + 5); break;
                        case 2: SelectorMedio(bselect, fselect, this._Anio, 6, this._PosX + 29, this._PosY + 5); break;
                    }
                }
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    calendario = new int[6, 7];
                    CrearMatrizCalendario(ref calendario);
                    InsertCalendar(ref calendario, ref this._Dia);

                    OUT.PrintLine("", fore, back, 0, 0);
                    tecla = keydata.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals("TAB"))) && (!(tecla.Equals("SHIFT + TAB"))) && (!(tecla.Equals(""))))
                    {
                        d = Convert.ToInt32(this._Dia);
                        tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));

                        if (pos == 0)
                        {
                            if (tecla.Equals("RIGHTARROW"))
                            {
                                d++;
                                if (d > tope_dia)
                                {
                                    d = 1;
                                    m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                    m++;
                                    if (m > 12)
                                    {
                                        m = 1;
                                        a = Convert.ToInt32(this._Anio);
                                        a++;
                                        this._Anio = a.ToString();
                                    }
                                    this._Mes = Convert.ToString(ConvertMes(Convert.ToInt32(m)));           
                                }
                                this._Dia = InsertDayStr(d);
                            }
                            if (tecla.Equals("LEFTARROW"))
                            {
                                d--;
                                if (d < 1)
                                {
                                    m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                    m--;
                                    
                                    if (m < 1)
                                    {
                                        m = 12;
                                        a = Convert.ToInt32(this._Anio);
                                        a--;
                                        if (a < 1500)
                                        {
                                            a = DateTime.Now.Year;
                                        }
                                        this._Anio = a.ToString();
                                    }
                                    this._Mes = Convert.ToString(ConvertMes(Convert.ToInt32(m)));
                                    d = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                }
                                this._Dia = InsertDayStr(d);
                            }
                            if (tecla.Equals("DOWNARROW"))
                            {
                                k = 0; g = 0;
                                while((calendario[k,g]) != d)
                                {
                                    g++;
                                    if (g == 7) 
                                    { 
                                        k++; 
                                        g = 0;
                                    }
                                }
                                if ((calendario[k + 1, g]) == 0)
                                {
                                    k = 0;
                                    while (((calendario[k + 1, g]) == 0) && (k < 6))
                                    {
                                        k++;
                                    }
                                    d = (calendario[k, g]);
                                } else { d = (calendario[k + 1, g]); }
                                this._Dia = InsertDayStr(d);
                            }
                            if (tecla.Equals("UPARROW"))
                            {
                                k = 0; g = 0;
                                while ((calendario[k, g]) != d)
                                {
                                    g++;
                                    if (g == 7)
                                    {
                                        k++;
                                        g = 0;
                                    }
                                }
                                if (((k - 1) == -1) || ((calendario[k - 1, g]) == 0)) 
                                {
                                    k = 5;
                                    while (((calendario[k, g]) == 0) && (k >= 0))
                                    {
                                        k--;
                                    }
                                    d = (calendario[k, g]);
                                }
                                else { d = (calendario[k - 1, g]); }
                                this._Dia = InsertDayStr(d);
                            }
                        }
                        if (pos == 1)
                        {
                            if (tecla.Equals("RIGHTARROW"))
                            {
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                m++;
                                if (m > 12)
                                {
                                    m = 1;
                                }
                                this._Mes = Convert.ToString(ConvertMes(Convert.ToInt32(m)));
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
                                if (d > tope_dia) { d = tope_dia; }
                                this._Dia = InsertDayStr(d);
                            }
                            if (tecla.Equals("LEFTARROW"))
                            {
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                m--;
                                if (m < 1)
                                {
                                    m = 12;
                                }
                                this._Mes = Convert.ToString(ConvertMes(Convert.ToInt32(m)));
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
                                if (d > tope_dia) { d = tope_dia; }
                                this._Dia = InsertDayStr(d);
                            }
                            if ((tecla.Equals("DOWNARROW")) || (tecla.Equals("UPARROW")))
                            {
                                Console.Beep(500, 200);
                            }
                        }
                        if (pos == 2)
                        {
                            if (tecla.Equals("DOWNARROW"))
                            {
                                a = Convert.ToInt32(this._Anio);
                                a++;
                                this._Anio = a.ToString();                                
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
                                if (d > tope_dia) { d = tope_dia; }
                                this._Dia = InsertDayStr(d);
                            }
                            if (tecla.Equals("UPARROW"))
                            {
                                a = Convert.ToInt32(this._Anio);
                                a--;
                                if (a == 1499) { a = DateTime.Now.Year; }
                                this._Anio = a.ToString();
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
                                if (d > tope_dia) { d = tope_dia; }
                                this._Dia = InsertDayStr(d);
                            }
                            if ((tecla.Equals("RIGHTARROW")) || (tecla.Equals("LEFTARROW")))
                            {
                                Console.Beep(500, 200);
                            }
                        }
                    } else
                    {
                        if (tecla.Equals("TAB"))
                        {
                            pos++;
                            if (pos == 3) { pos = 0; }
                        }
                        else
                        {
                            if (tecla.Equals("SHIFT + TAB"))
                            {
                                pos--;
                                if (pos == -1) { pos = 2; }
                            }
                            else 
                            { 
                                if (tecla.Equals("ENTER")) 
                                {
                                    string tr = "";
                                    for (int p = 0; p < 38; p++) { tr += " "; }
                                    for (int p = 0; p < 30; p++)
                                    {
                                        OUT.PrintLine(tr, fore, back, this._PosX, this._PosY + 9 + p);
                                    }
                                    estado = false; 
                                } 
                            }
                        } 
                    }
                }
                else { estado = false; }
            }
        }
        public object GetDataInfo()
        {
            int fecha = Convert.ToInt32(this._Anio);
            fecha *= 100;
            fecha += Convert.ToInt32(ConvertMes(this._Mes.ToString()));
            fecha *= 100;
            fecha += Convert.ToInt32(this._Dia);
            return (fecha);
        }
        public void SetDataInfo(object dataInfo)
        {
            this._Anio = ((Convert.ToInt32(dataInfo)) / 10000).ToString();
            int k = (((Convert.ToInt32(dataInfo)) % 10000) / 100);            
            this._Mes = ConvertMes(k).ToString();
            k = (((Convert.ToInt32(dataInfo)) % 10000) % 100);
            if (k < 10) { this._Dia = "0"; }
            else { this._Dia = ""; }
            this._Dia = String.Concat(this._Dia, k.ToString());
        }
        void FuncIOData.SetTypeDataIN(TypeDataIN cond)
        {
            throw new NotImplementedException();
        }
        private string InsertDayStr(int d)
        {
            string day = "";
            if (d < 10)
            {
                day = "0";
            }
            day = String.Concat(day, d.ToString());
            return day;
        }
        private void InsertCalendar(ref int[,] calendario, ref string dia)
        {
            int d = Convert.ToInt32(dia);
            int k = 0, condicion_color = 0, valor = 0;
            string kl = "";
            color bpointer = color.none;
            color fpointer = color.none;

            for (int j = 0; j < 6; j++)
            {
                for(int i = 0; i < 7; i++)
                {
                    valor = calendario[j, i];
                    kl = "";
                    condicion_color = 0;
                    if (valor > 0)
                    {
                        if (valor == d)
                        {
                            condicion_color++;
                        }                        
                        if (valor < 10) { kl = String.Concat("  ", valor.ToString(), " "); }
                        else { kl = String.Concat(" ", valor.ToString(), " "); }                                                
                    } else { kl = "    "; }
                    bpointer = this._BackPointer[condicion_color];
                    fpointer = this._ForePointer[condicion_color];
                    SelectorMedio(bpointer, fpointer, kl, 2, this._PosX + 2 + (i * 5), this._PosY + 14 + (k * 4));
                }
                k++;
            }
        }
        private void CrearMatrizCalendario(ref int[,] matriz)
        {
            int A = Modulo_A();
            int B = Modulo_B();
            int C = Modulo_C();
            int D = Modulo_D();
            int KONS = (A + B + C + D + 1) % 7;
            int tope_mes = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
            int k = 1;
            for(int t = 0; t < KONS; t++)
            {
                matriz[0,t] = 0;
            }
            for(int j = 0; j < 6; j++)
            {
                for(int i = KONS; i < 7; i++)
                {
                    if (k <= tope_mes)
                    {
                        matriz[j, i] = k;
                        k++;
                    } else { matriz[j, i] = 0; }                    
                }
                KONS = 0;
            }
        }
        private int Modulo_D()
        {
            int[] constante = { 6, 2, 2, 5, 0, 3, 5, 1, 4, 6, 2, 4 };
            int valor = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
            return constante[valor - 1];
        }
        private int Modulo_C()
        {
            int res = 0;
            bool bisiesto = Bisiesto(Convert.ToInt32(this._Anio));
            if ((bisiesto) && ((this._Mes.Equals("    Enero     ")) || (this._Mes.Equals("   Febrero    ")))) {
                res = -1;
            }
            return (res);
        }
        private int Modulo_B()
        {
            int anio = Convert.ToInt32(this._Anio);
            int k = anio % 100;
            int l = (int)((Convert.ToDouble(k)) / 4);
            return (k + l);
        }
        private int Modulo_A()
        {
            int anio = Convert.ToInt32(this._Anio);
            int res = 0;
            int tope = 0;

            if (anio >= 2000)
            {
                res = 0;
                tope = 2000;
                while(anio > ((tope + 100) - 1))
                {
                    res -= 2;
                    tope += 100;
                }
            }
            if (anio < 2000)
            {
                res = 1;
                tope = 2000;
                while (anio < (tope - 100))
                {
                    res += 2;
                    tope -= 100;
                }
            }
            return (res);
        }
    }
}
