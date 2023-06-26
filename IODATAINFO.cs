using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{    
    public class IODATAINFO : IOdata , FuncIOData
    {
        private string _Titulo;
        private string _DataVisual;
        private string _DataInfo;
        private color[] _BackCorral;
        private color[] _ForeCorral;
        private color[] _Backtitulo;
        private color[] _Foretitulo;
        private color[] _BackData;
        private color[] _ForeData;
        private TypePost _Position;
        private TypeLine _Line;
        private TypeINFO _CondicionVisual;
        private int _TypeKey;
        private int _Width;
        private int _Heinght;
        private int _WidthData;
        private int _SizeDigitValue;
        private int _CountDigit;
        private int _PosX;
        private int _PosY;
        public IODATAINFO(string titulo, color[] backCorral, color[] foreCorral,
                          color[] backtitulo, color[] foretitulo, color[] backData, 
                          color[] foreData, TypePost pos, TypeLine linea, TypeINFO condicionvisual, 
                          int widthData, int cantdigitvalue, int px, int py) : base() {
            this._Titulo = titulo;
            this._DataVisual = "";
            this._DataInfo = "";
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._Backtitulo = backtitulo;
            this._Foretitulo = foretitulo;
            this._BackData = backData;
            this._ForeData = foreData;
            this._Position = pos;
            this._Line = linea;
            this._CondicionVisual = condicionvisual;
            this._TypeKey = 0;
            this._Width = CalculadorWidht(pos, titulo.Length, widthData, cantdigitvalue);
            this._Heinght = CalculadorHeingth(pos, titulo.Length, widthData, cantdigitvalue);
            this._WidthData = widthData;
            this._SizeDigitValue = cantdigitvalue;
            this._CountDigit = 0;
            this._PosX = px;
            this._PosY = py;            
        }
        public void Display(color back, color fore)
        {
            bool estado = true, script = false;
            color bcorral = color.none;
            color fcorral = color.none;
            color btitulo = color.none;
            color ftitulo = color.none;
            color bdatinf = color.none;
            color fdatinf = color.none;
            int postituloX = 0, postituloY = 0, poswidthdataX = 0, poswidthdataY = 0;
            int posdatavisualX = 0, posdatavisualY = 0;
            IN keydata = new IN();

            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                ConfiguradorKeyInfo(ref keydata, this._TypeKey);
            }
            
            while (estado)
            {
                bcorral = this._BackCorral[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];
                fcorral = this._ForeCorral[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];
                btitulo = this._Backtitulo[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];
                ftitulo = this._Foretitulo[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];
                bdatinf = this._BackData[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];
                fdatinf = this._ForeData[(int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString())];

                switch(this._Position)
                {
                    case (TypePost._LEFT): CalcularLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._UP_LEFT): CalcularUpLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._UP_CENTER): CalcularUpCenter(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._Width, this._PosX, this._PosY); break;
                    case (TypePost._DOWN_LEFT): CalcularDownLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._DOWN_CENTER): CalcularDownCenter(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._Width, this._PosX, this._PosY); break;
                }
                
                DRAW.TablaLine(this._Line, bcorral, fcorral, new int[] { this._Width }, new int[] { this._Heinght }, this._PosX, this._PosY);
                DRAW.CuadradoSolid(bdatinf, this._WidthData, 1, poswidthdataX, poswidthdataY);
                OUT.PrintLine(this._Titulo, ftitulo, btitulo, postituloX, postituloY);
                OUT.PrintLine(this._DataVisual, fdatinf, bdatinf, posdatavisualX, posdatavisualY);

                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = keydata.InputMode();

                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals("BACKSPACE"))) && (!(tecla.Equals(""))))
                    {
                        if (this._CountDigit < this._SizeDigitValue)
                        {
                            script = VerifIngresoTecla(ref tecla, this._TypeKey);
                            if (script)
                            {
                                script = false;
                                this._DataInfo = String.Concat(this._DataInfo, tecla);
                                if (this._CondicionVisual == TypeINFO._PASSWORD)
                                {
                                    tecla = "*";
                                }
                                this._DataVisual = String.Concat(this._DataVisual, tecla);
                                this._CountDigit++;
                            }
                        } else { Console.Beep(600, 100); }
                    } else {
                        if (tecla.Equals("BACKSPACE"))
                        {
                            if (this._CountDigit > 0)
                            {
                                OUT.PrintLine(" ", fdatinf, bdatinf, posdatavisualX + this._DataVisual.Length - 1, posdatavisualY);
                                this._DataVisual = this._DataVisual.Remove(this._DataVisual.Length - 1);
                                this._DataInfo = this._DataInfo.Remove(this._DataInfo.Length - 1);
                                this._CountDigit--;
                            } else { Console.Beep(600, 100); }
                        } else {
                            if (tecla.Equals("ENTER")) { estado = false; }
                        }
                    }                    
                } else { estado = false; }                
            }                        
        }
        public void SetTypeDataIN(TypeDataIN cond)
        {
            int bitdata = ((int)cond);
            this._TypeKey |= (1 << bitdata);
        }
        public object GetDataInfo()
        {
            return ((object)this._DataInfo);
        }
        public void SetDataInfo(object dato)
        {
            string info = dato.ToString();
            this._CountDigit = info.Length;
            this._DataInfo = info;
            if (this._CondicionVisual == TypeINFO._PASSWORD)
            {
                for(int i = 0; i < this._CountDigit; i++)
                {
                    this._DataVisual += "*";
                }
            } else { this._DataVisual = this._DataInfo; }
        }
//----------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------
        private int CalculadorHeingth(TypePost cond, int titulo, int widthData, int cantdigitvalue)
        {
            int ft = 0;
            if (cond == TypePost._LEFT) { ft = 4; }
            if ((cond == TypePost._UP_LEFT) || (cond == TypePost._UP_CENTER) || (cond == TypePost._DOWN_LEFT) || (cond == TypePost._DOWN_CENTER)) { ft = 7; }
            return (ft);
        }
        private int CalculadorWidht(TypePost cond, int titulo, int widthData, int cantdigitvalue)
        {
            int ft = 0;
            if (cond == TypePost._LEFT)
            {
                ft = titulo + 3;
                if (widthData >= cantdigitvalue) { ft += widthData; }
                if (widthData < cantdigitvalue) { ft += cantdigitvalue; }
                ft += 3;
            }
            if ((cond == TypePost._UP_LEFT) || (cond == TypePost._UP_CENTER) || (cond == TypePost._DOWN_LEFT) || (cond == TypePost._DOWN_CENTER))
            {
                if ((titulo > widthData) && (titulo > cantdigitvalue)) { ft = titulo + 3; }
                if ((widthData > titulo) && (widthData > cantdigitvalue)) { ft = widthData + 3; }
                if ((cantdigitvalue > titulo) && (cantdigitvalue > widthData)) { ft = cantdigitvalue + 3; }
            }
            return (ft);
        }
        private void CalcularLeft(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int x, int y)
        {
            postituloX = x + 2; 
            postituloY = y + 2;
            poswidthdataX = x + 4 + this._Titulo.Length;
            poswidthdataY = y + 1;
            posdatavisualX = postituloX + titulo + 3;
            posdatavisualY = postituloY;
        }
        private void CalcularUpLeft(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int x, int y)
        {
            postituloX = x + 2;
            postituloY = y + 2;
            poswidthdataX = x + 1;
            poswidthdataY = y + 4;
            posdatavisualX = postituloX + 1;
            posdatavisualY = postituloY + 3;
        }
        private void CalcularUpCenter(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int width, int x, int y)
        {
            postituloX = x + ((width - titulo) / 2);
            postituloY = y + 2;
            poswidthdataX = x + 1;
            poswidthdataY = y + 4;
            posdatavisualX = x + 2;
            posdatavisualY = y + 5;
        }
        private void CalcularDownLeft(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int x, int y)
        {
            postituloX = x + 2;
            postituloY = y + 5;
            poswidthdataX = x + 1;
            poswidthdataY = y + 1;
            posdatavisualX = x + 2;
            posdatavisualY = y + 2;
        }
        private void CalcularDownCenter(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int width, int x, int y)
        {
            postituloX = x + ((width - titulo) / 2); 
            postituloY = y + 5;
            poswidthdataX = x + 1;
            poswidthdataY = y + 1;
            posdatavisualX = x + 2;
            posdatavisualY = y + 2;
        }      
    }
}
