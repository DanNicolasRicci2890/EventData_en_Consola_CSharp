using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IODATETIME : IOdata, FuncIOData
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
        private TypeLine _Linea;

        private int _PosX;
        private int _PosY;

        public IODATETIME(string titulo, color[] backCorral, color[] foreCorral, color[] backTitulo, color[] foreTitulo, color[] backSelect, color[] foreSelect, TypeLine lp, int posX, int posY)
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
            this._Linea = lp;
            this._PosX = posX;
            this._PosY = posY;
        }

        public void Display(color back, color fore)
        {
            bool estado = true;            
            int condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
            color bcorral = this._BackCorral[condicion_color];
            color fcorral = this._ForeCorral[condicion_color];
            color btitulo = this._BackTitulo[condicion_color];
            color ftitulo = this._ForeTitulo[condicion_color];
            IN keydata = new IN();
            string tecla = "";
            int i = 0, pos = 0;

            
            DRAW.TablaLine(this._Linea, bcorral, fcorral, new int[] { 60 }, new int[] { 3 , 4 }, this._PosX, this._PosY);
            SelectorMedio(btitulo, ftitulo, this._Titulo, 57, (this._PosX + 1), (this._PosY + 1));

            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                keydata.SetCondIN(INCond._ARROWS);
                keydata.SetCondIN(INCond._ENTER);
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

                    switch(i)
                    {
                        case 0: SelectorMedio(bselect, fselect, this._Dia, 4, this._PosX + 10, this._PosY + 5); break;
                        case 1: SelectorMedio(bselect, fselect, this._Mes, 14, this._PosX + 21, this._PosY + 5); break;
                        case 2: SelectorMedio(bselect, fselect, this._Anio, 6, this._PosX + 42, this._PosY + 5); break;
                    }
                }
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    tecla = keydata.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals(""))))
                    {
                        int d = 0, m = 0, a = 0;
                        int tope_dia = 0;
                        if (tecla.Equals("UPARROW"))
                        {
                            if (pos == 0)
                            {
                                d = Convert.ToInt32(this._Dia);
                                d--;
                                if (d < 1)
                                {
                                    m = Convert.ToInt32(ConvertMes(this._Mes));
                                    m--;
                                    if (m < 1)
                                    {
                                        m = 12;
                                        a = Convert.ToInt32(this._Anio);
                                        a--;
                                        if (a == 1499) { a = DateTime.Now.Year; }
                                        this._Anio = a.ToString();
                                    }
                                    this._Mes = Convert.ToString(ConvertMes(m));
                                    d = TopeDia(m, Convert.ToInt32(this._Anio));
                                }
                                this._Dia = d.ToString();
                            }
                            if (pos == 1)
                            {
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                m--;
                                if (m < 1)
                                {
                                    m = 12;
                                    a = Convert.ToInt32(this._Anio);
                                    a--;
                                    if (a == 1499) { a = DateTime.Now.Year; }
                                    this._Anio = a.ToString();
                                }
                                this._Mes = Convert.ToString(ConvertMes(Convert.ToInt32(m)));
                            }
                            if (pos == 2)
                            {
                                a = Convert.ToInt32(this._Anio);
                                a--;
                                if (a == 1499) { a = DateTime.Now.Year; }
                                this._Anio = a.ToString();
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                if (m == 2)
                                {
                                    d = Convert.ToInt32(this._Dia);
                                    tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                    if (d > tope_dia)
                                    {
                                        d = tope_dia;
                                        this._Dia = d.ToString();
                                    }
                                }
                            }
                        }
                        if (tecla.Equals("DOWNARROW"))
                        {
                            if (pos == 0)
                            {
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
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
                                        this._Anio = Convert.ToString(a);
                                    }
                                    this._Mes = Convert.ToString(ConvertMes(m));
                                }
                                if (d < 10)
                                {
                                    this._Dia = "0";
                                    this._Dia = String.Concat(this._Dia, d.ToString());
                                }
                                else
                                {
                                    this._Dia = d.ToString();
                                }
                            }
                            if (pos == 1)
                            {
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                m++;
                                if (m > 12)
                                {
                                    m = 1;
                                    a = Convert.ToInt32(this._Anio);
                                    a++;
                                    this._Anio = a.ToString();
                                }
                                this._Mes = Convert.ToString(ConvertMes(m));
                                tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                d = Convert.ToInt32(this._Dia);
                                if (d > tope_dia)
                                {
                                    d = tope_dia;
                                    this._Dia = d.ToString();
                                }
                            }
                            if (pos == 2)
                            {
                                a = Convert.ToInt32(this._Anio);
                                a++;
                                this._Anio = Convert.ToString(a.ToString());
                                m = Convert.ToInt32(ConvertMes(this._Mes.ToString()));
                                if (m == 2)
                                {
                                    d = Convert.ToInt32(this._Dia);
                                    tope_dia = TopeDia(Convert.ToInt32(ConvertMes(this._Mes.ToString())), Convert.ToInt32(this._Anio));
                                    if (d > tope_dia)
                                    {
                                        d = tope_dia;
                                    }
                                    this._Dia = d.ToString();
                                }
                            }
                        }
                        if (tecla.Equals("RIGHTARROW"))
                        {
                            pos++;
                            if (pos > 2) { pos = 0; }
                        }
                        if (tecla.Equals("LEFTARROW"))
                        {
                            pos--;
                            if (pos < 0) { pos = 2; }
                        }
                        
                    }
                    else { if (tecla.Equals("ENTER")) { estado = false; } }
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
        public int CalcularBoxWidth()
        {
            int k = 0;
            if (this._StateEvent != TypeStateIO._ACTIVATED)
            {
                k = 5;
            }
            return (k);
        }
    }
}
