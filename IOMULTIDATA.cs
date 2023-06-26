using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IOMULTIDATA : IOdata , FuncIOData
    {
        private string _Titulo;
        private string _Separador;
        private string[] _DataVisual;
        private string[] _DataInfo;
        private color[] _BackCorral;
        private color[] _ForeCorral;
        private color[] _Backtitulo;
        private color[] _Foretitulo;
        private color[] _BackData;
        private color[] _ForeData;
        private TypePost _Position;
        private TypeLine _Line;
        private TypeINFO _CondicionVisual;
        private int[] _Width;
        private int[] _CountDigit;
        private int _TypeKey;
        private int _PosX;
        private int _PosY;

        public IOMULTIDATA(string titulo,
                        string separador,   
                      color[] backCorral, 
                      color[] foreCorral, 
                      color[] backtitulo, 
                      color[] foretitulo, 
                        color[] backData, 
                        color[] foreData, 
                       TypePost position, 
                           TypeLine line, 
                TypeINFO condicionVisual, 
                             int[] width, 
                                int posX, 
                                int posY) : base()
        {
            this._Titulo = titulo;
            this._Separador = separador;
            this._DataVisual = new string[width.Length];
            this._DataInfo = new string[width.Length];
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._Backtitulo = backtitulo;
            this._Foretitulo = foretitulo;
            this._BackData = backData;
            this._ForeData = foreData;
            this._Position = position;
            this._Line = line;
            this._CondicionVisual = condicionVisual;
            this._Width = width;
            this._CountDigit = new int[width.Length];            
            this._TypeKey = 0;
            this._PosX = posX;
            this._PosY = posY;
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
            int widthbox = CalculadorWidht(this._Position, this._Titulo.Length, this._Width);
            int heinghtbox = CalculadorHeinght(this._Position);
            int condicion_color = 0, pos_script = 0, pdvx = 0;
            IN keydata = new IN();

            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                ConfiguradorKeyInfo(ref keydata, this._TypeKey);
                keydata.SetCondIN(INCond._TAB);
            }

            while (estado)
            {
                condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
                
                bcorral = this._BackCorral[condicion_color];
                fcorral = this._ForeCorral[condicion_color];
                btitulo = this._Backtitulo[condicion_color];
                ftitulo = this._Foretitulo[condicion_color];                                

                switch (this._Position)
                {
                    case (TypePost._LEFT): CalcularLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._UP_LEFT): CalcularUpLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._UP_CENTER): CalcularUpCenter(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, widthbox, this._PosX, this._PosY);  break;
                    case (TypePost._DOWN_LEFT): CalcularDownLeft(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, this._PosX, this._PosY); break;
                    case (TypePost._DOWN_CENTER): CalcularDownCenter(ref postituloX, ref postituloY, ref poswidthdataX, ref poswidthdataY, ref posdatavisualX, ref posdatavisualY, this._Titulo.Length, widthbox, this._PosX, this._PosY); break;
                }

                DRAW.TablaLine(this._Line, bcorral, fcorral, new int[] { widthbox }, new int[] { heinghtbox }, this._PosX, this._PosY);
                OUT.PrintLine(this._Titulo, ftitulo, btitulo, postituloX, postituloY);
                for(int i = 0; i < this._Width.Length; i++)
                {
                    condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
                    if ((this._StateEvent == TypeStateIO._ACTIVATED) && (pos_script == i))
                    {
                        condicion_color += 2;
                    }
                    bdatinf = this._BackData[condicion_color];
                    DRAW.CuadradoSolid(bdatinf, this._Width[i], 1, poswidthdataX, poswidthdataY);
                    poswidthdataX += this._Width[i];
                    poswidthdataX += 3;
                }
                pdvx = posdatavisualX;
                for (int i = 0; i < this._DataVisual.Length; i++)
                {
                    condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
                    if ((this._StateEvent == TypeStateIO._ACTIVATED) && (pos_script == i))
                    {
                        condicion_color += 2;
                    }
                    bdatinf = this._BackData[condicion_color];
                    fdatinf = this._ForeData[condicion_color];                    
                    OUT.PrintLine(this._DataVisual[i], fdatinf, bdatinf, pdvx, posdatavisualY);
                    pdvx += this._Width[i];
                    pdvx += 3;
                }
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = keydata.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals("BACKSPACE"))) && (!(tecla.Equals(""))) && (!(tecla.Equals("TAB"))))
                    {
                        if (this._CountDigit[pos_script] < this._Width[pos_script])
                        {
                            script = VerifIngresoTecla(ref tecla, this._TypeKey);
                            if (script)
                            {
                                script = true;
                                this._DataInfo[pos_script] = String.Concat(this._DataInfo[pos_script], tecla);
                                if (this._CondicionVisual == TypeINFO._PASSWORD)
                                {
                                    tecla = "*";
                                }
                                this._DataVisual[pos_script] = String.Concat(this._DataVisual[pos_script], tecla);
                                this._CountDigit[pos_script]++;
                            }
                        } else { Console.Beep(600, 100); }
                    } else {
                        if (tecla.Equals("BACKSPACE"))
                        {
                            if (this._CountDigit[pos_script] > 0)
                            {
                                this._CountDigit[pos_script]--;
                                pdvx = posdatavisualX;
                                for (int i = 0; i < pos_script; i++)
                                {
                                    pdvx += this._Width[i];
                                    pdvx += 3;
                                }
                                pdvx += this._CountDigit[pos_script];
                                OUT.PrintLine(" ", fdatinf, bdatinf, pdvx, posdatavisualY);
                                this._DataVisual[pos_script] = this._DataVisual[pos_script].Remove(this._DataVisual[pos_script].Length - 1);
                                this._DataInfo[pos_script] = this._DataInfo[pos_script].Remove(this._DataInfo[pos_script].Length - 1);                                
                            } else { Console.Beep(600, 100); }
                        } else {
                            if (tecla.Equals("TAB"))
                            {
                                pos_script++;
                                if (pos_script == this._Width.Length)
                                {
                                    pos_script = 0;
                                }
                            } else {
                                if (tecla.Equals("ENTER")) { estado = false; }
                            }
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
            string info = "";

            for(int i = 0; i < this._DataInfo.Length; i++)
            {
                info = String.Concat(info, this._DataInfo[i]);
                if (i < this._DataInfo.Length - 1)
                {
                    info = String.Concat(info, ((this._Separador.ToCharArray())[i]).ToString());
                }                
            }

            return ((object)info);
        }
        public void SetDataInfo(object dato)
        {            
            string valor = "";
            char[] lit = this._Separador.ToCharArray();
            string[] info = (dato.ToString()).Split(lit);
            int i = 0;

            while((i < this._Width.Length) && (i < info.Length))
            {
                valor = info[i].ToString();
                while(valor.Length > this._Width[i])
                {
                    valor = valor.Remove(valor.Length - 1);
                }
                this._DataInfo[i] = valor;
                this._CountDigit[i] = valor.Length;
                if (this._CondicionVisual == TypeINFO._PASSWORD)
                {
                    for(int j = 0; j < valor.Length; j++)
                    {
                        this._DataVisual[i] += "*"; ;
                    }
                }
                if (this._CondicionVisual != TypeINFO._PASSWORD)
                {
                    this._DataVisual[i] = String.Copy(valor);
                }
                valor = "";
                i++;
            }           
        }
        private void CalcularLeft(ref int postituloX, ref int postituloY, ref int poswidthdataX, ref int poswidthdataY, ref int posdatavisualX, ref int posdatavisualY, int titulo, int x, int y)
        {
            postituloX = x + 2;
            postituloY = y + 2;
            poswidthdataX = x + 4 + titulo;
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
            posdatavisualX = postituloX;
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
        private int CalculadorHeinght(TypePost cond)
        {
            int h = 0;
            if (cond == TypePost._LEFT) { h = 4; }
            if ((cond == TypePost._DOWN_LEFT) || (cond == TypePost._DOWN_CENTER) || (cond == TypePost._UP_LEFT) || (cond == TypePost._UP_CENTER)) { h = 7; }
            return h;
        }
        private int CalculadorWidht(TypePost cond, int titulo, int[] widthData)
        {
            int w = 0;
            if (cond == TypePost._LEFT)
            {
                w = 4 + titulo;
                for(int i = 0; i < widthData.Length; i++)
                {
                    w += widthData[i];
                    w += 3;
                }                
            }
            if ((cond == TypePost._DOWN_LEFT) || (cond == TypePost._DOWN_CENTER) || (cond == TypePost._UP_LEFT) || (cond == TypePost._UP_CENTER))
            {
                for (int i = 0; i < widthData.Length; i++)
                {
                    w += widthData[i];
                    w += 3;
                }
                if (w < titulo) { w = titulo + 2; }
            }
            return w;
        }
    }
}

