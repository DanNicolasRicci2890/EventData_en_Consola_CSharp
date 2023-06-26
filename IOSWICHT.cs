using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IOSWICHT : IOdata , FuncIOData
    {
        private string _Titulo;
        private string[] _SwitchSelect;
        private int _RolesPermisos;
        private color[] _BackCorral;
        private color[] _ForeCorral;
        private color[] _Backtitulo;
        private color[] _Foretitulo;
        private color[] _BackDato;
        private color[] _ForeDato;
        private TypeLine _Line;
        private TypePost _Position;
        private int _PosX;
        private int _PosY;

        public IOSWICHT(string titulo, string[] switchSelect, color[] backCorral, color[] foreCorral, color[] backtitulo, color[] foretitulo, color[] backdato, color[] foredato, TypeLine line, TypePost position, int posX, int posY)
        {
            this._Titulo = titulo;
            this._SwitchSelect = switchSelect;
            this._RolesPermisos = 0;
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._Backtitulo = backtitulo;
            this._Foretitulo = foretitulo;
            this._BackDato = backdato;
            this._ForeDato = foredato;
            this._Line = line;
            this._Position = position;
            this._PosX = posX;
            this._PosY = posY;
        }
        public void Display(color back, color fore)
        {
            bool estado = true, script = false;
            int widthbox = CalcularBoxWidth(), heinghtbox = CalcularBoxHeingth();
            int condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
            int postituloX = CalcularTituloX(), postituloY = CalcularTituloY(), pos = 0;
            int posDataX = CalcularDataX(widthbox), posDataY = CalcularDataY(), grosor = CalcularMaximo();
            color bcorral = this._BackCorral[condicion_color];
            color fcorral = this._ForeCorral[condicion_color];
            color btitulo = this._Backtitulo[condicion_color];
            color ftitulo = this._Foretitulo[condicion_color];
            IN keydata = new IN();
            
            // imprimir caja
            DRAW.TablaLine(this._Line, bcorral, fcorral, new int[] { widthbox }, new int[] { heinghtbox }, this._PosX, this._PosY);
            // titulo
            Selector(btitulo, ftitulo, this._Titulo, this._Titulo.Length, postituloX, postituloY);

            while ((pos < this._SwitchSelect.Length) && (((this._RolesPermisos >> pos) & 1) == 0)) pos++;


            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                keydata.SetCondIN(INCond._ARROWS);
                keydata.SetCondIN(INCond._TAB);
                keydata.SetCondIN(INCond._ENTER);
            }

            while (estado) 
            {
                condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    if (((this._RolesPermisos >> pos) & 1) == 1) { condicion_color++; }
                }
                color bdata = this._BackDato[condicion_color];
                color fdata = this._ForeDato[condicion_color];
                SelectorMedio(bdata, fdata, this._SwitchSelect[pos], grosor, posDataX, posDataY);

                if (script)
                {
                    script = false;
                    System.Threading.Thread.Sleep(500);
                }

                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = keydata.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals("TAB"))) && (!(tecla.Equals(""))))
                    {
                        //    incrementar
                        if ((tecla.Equals("RIGHTARROW")) || (tecla.Equals("DOWNARROW"))) { pos++; }
                        //    decrementar
                        if ((tecla.Equals("UPARROW")) || (tecla.Equals("LEFTARROW"))) { pos--; }
                        if (pos == this._SwitchSelect.Length) { pos = 0; }
                        if (pos == -1) { pos = this._SwitchSelect.Length - 1; }
                    }
                    else
                    {
                        if (tecla.Equals("TAB"))
                        {
                            if (((this._RolesPermisos >> pos) & 1) == 0)
                            {
                                this._RolesPermisos = 0;
                                this._RolesPermisos = this._RolesPermisos | (1 << pos);
                                script = true;
                            }
                        } else { if (tecla.Equals("ENTER")) { estado = false; } }
                    }
                } else { estado = false; }                    
            }
        }

        public object GetDataInfo() => this._RolesPermisos;
        public void SetDataInfo(object dataInfo) => this._RolesPermisos = Convert.ToInt32(dataInfo);
        void FuncIOData.SetTypeDataIN(TypeDataIN cond)
        {
            throw new NotImplementedException();
        }
        private int CalcularMaximo()
        {
            int valor = 0;
            for (int i = 0; i < this._SwitchSelect.Length; i++)
            {
                if (valor < this._SwitchSelect[i].Length)
                {
                    valor = this._SwitchSelect[i].Length;
                }
            }
            return (valor);
        }
        private int CalcularBoxWidth()
        {
            int valor = 0;
            if (this._Position == TypePost._LEFT) { valor = CalcularMaximo() + this._Titulo.Length + 10; }
            else
            {
                valor = CalcularMaximo() + 4;
                if (valor < this._Titulo.Length) { valor = this._Titulo.Length + 4; }
            }            
            return (valor);
        }
        private int CalcularBoxHeingth()
        {
            int valor = 8;
            if (this._Position == TypePost._LEFT) { valor = 6; }
            return (valor);
        }
        private int CalcularTituloX()
        {
            int r = 0;
            if (this._Position == TypePost._LEFT) { r = this._PosX + 2; }
            if ((this._Position == TypePost._UP_LEFT) || (this._Position == TypePost._DOWN_LEFT)) 
            { r = this._PosX + 1; }
            if ((this._Position == TypePost._UP_CENTER) || (this._Position == TypePost._DOWN_CENTER)) 
            { r = this._PosX + ((CalcularBoxWidth() - this._Titulo.Length) / 2); }
            return r;
        }
        private int CalcularTituloY()
        {
            int r = 0;
            if (this._Position == TypePost._LEFT) { r = this._PosY + 2; }
            if ((this._Position == TypePost._UP_LEFT) || (this._Position == TypePost._UP_CENTER))
            { r = this._PosY + 1; }
            if ((this._Position == TypePost._DOWN_LEFT) || (this._Position == TypePost._DOWN_CENTER))
            { r = this._PosY + 5; }
            return r;
        }
        private int CalcularDataX(int w)
        {
            int r = 0;
            if (this._Position == TypePost._LEFT) { r = this._PosX + 6 + this._Titulo.Length; }
            else { r = this._PosX + ((w - CalcularMaximo()) / 2); }
            return r;
        }
        private int CalcularDataY()
        {
            int r = 0;
            if (this._Position == TypePost._LEFT) { r = this._PosY + 2; }
            if ((this._Position == TypePost._UP_LEFT) || (this._Position == TypePost._UP_CENTER)) 
            {
                r = this._PosY + 5;
            }
            if ((this._Position == TypePost._DOWN_LEFT) || (this._Position == TypePost._DOWN_CENTER)) 
            {
                r = this._PosY + 1;
            }
            return r;
        }
    }
}
