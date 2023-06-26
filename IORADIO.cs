using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IORADIO : IOdata , FuncIOData
    {
        private string _Titulo;
        private string[] _CheckSelect;
        private int _RolesPermisos;

        private color[] _BackCorral;
        private color[] _ForeCorral;

        private color[] _Backtitulo;
        private color[] _Foretitulo;

        private color[] _BackSelect;
        private color[] _ForeSelect;

        private color[] _BackOption;
        private color[] _ForeOption;

        private TypeLine _Line;
        private int _Columnas;
        private int _PosX;
        private int _PosY;

        public IORADIO(string titulo, string[] checkSelect, color[] backCorral, color[] foreCorral, color[] backtitulo, color[] foretitulo, color[] backSelect, color[] foreSelect, color[] backOption, color[] foreOption, TypeLine line, int columnas, int posX, int posY)
        {
            this._Titulo = titulo;
            this._CheckSelect = checkSelect;
            this._RolesPermisos = 0;
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._Backtitulo = backtitulo;
            this._Foretitulo = foretitulo;
            this._BackSelect = backSelect;
            this._ForeSelect = foreSelect;
            this._BackOption = backOption;
            this._ForeOption = foreOption;
            this._Line = line;
            this._Columnas = columnas;
            this._PosX = posX;
            this._PosY = posY;
        }
        public void Display(color back, color fore)
        {
            bool estado = true, script = false;
            int postituloX = this._PosX + ((CalcularBoxWidth() - this._Titulo.Length) / 2);
            int postituloY = this._PosY + 1;
            int widthbox = CalcularBoxWidth(), heinghtbox = CalcularBoxHeingth();
            int condicion_color = 0, grosor = CalcularMaximo();
            int posOptX = 0, posOptY = 0, pos = 0, px = 0, py = 0;
            IN keydata = new IN();
            condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
            color bcorral = this._BackCorral[condicion_color];
            color fcorral = this._ForeCorral[condicion_color];
            color btitulo = this._Backtitulo[condicion_color];
            color ftitulo = this._Foretitulo[condicion_color];
            color bdatsel = color.none;
            color fdatsel = color.none;
            color bdatopt = color.none;
            color fdatopt = this._ForeOption[condicion_color];

            // imprimir caja
            DRAW.TablaLine(this._Line, bcorral, fcorral, new int[] { widthbox }, new int[] { heinghtbox }, this._PosX, this._PosY);

            // titulo
            Selector(btitulo, ftitulo, this._Titulo, this._Titulo.Length, postituloX, postituloY);

            while (estado)
            {
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    keydata.SetCondIN(INCond._ARROWS);
                    keydata.SetCondIN(INCond._TAB);
                    keydata.SetCondIN(INCond._ENTER);
                }
                posOptX = this._PosX + 3;
                posOptY = this._PosY + 5;
                for (int i = 0; i < this._CheckSelect.Length; i++)
                {
                    switch (this._StateEvent)
                    {
                        case (TypeStateIO._INACTIVATED): condicion_color = 0; break;
                        case (TypeStateIO._SEMIACTIVATED): condicion_color = 2; break;
                        case (TypeStateIO._ACTIVATED): condicion_color = 4; break;
                    }
                    if ((pos == i) && (this._StateEvent == TypeStateIO._ACTIVATED))
                    {
                        bdatsel = this._BackSelect[condicion_color + 2];
                        if (((this._RolesPermisos >> i) & 1) == 1)
                        {
                            bdatopt = this._BackOption[condicion_color + 1];
                            fdatsel = this._ForeSelect[condicion_color + 2];
                        }
                        if (((this._RolesPermisos >> i) & 1) == 0)
                        {
                            bdatopt = this._BackOption[condicion_color];
                            fdatsel = this._ForeSelect[condicion_color + 1];
                        }
                    }
                    else
                    {
                        if (((this._RolesPermisos >> i) & 1) == 1) { condicion_color++; }
                        bdatopt = this._BackOption[condicion_color];
                        fdatsel = this._ForeSelect[condicion_color];
                        bdatsel = this._BackSelect[condicion_color];
                    }                    
                    Checkeador(bcorral, fdatopt, bdatopt, posOptX, posOptY);
                    Selector(bdatsel, fdatsel, this._CheckSelect[i], grosor, posOptX + 3, posOptY);
                    if (((i + 1) % this._Columnas) == 0)
                    {
                        posOptX = this._PosX + 3;
                        posOptY += 4;
                    }
                    else { posOptX += grosor + 6; }
                    if (script)
                    {
                        script = false;
                        System.Threading.Thread.Sleep(200);
                    }
                }
                if (this._StateEvent == TypeStateIO._ACTIVATED)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = keydata.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals("TAB"))) && (!(tecla.Equals(""))))
                    {
                        switch (tecla)
                        {
                            case ("RIGHTARROW"): px++; break;
                            case ("LEFTARROW"): px--; break;
                            case ("DOWNARROW"): py += this._Columnas; break;
                            case ("UPARROW"): py -= this._Columnas; break;
                        }
                        if (px == this._Columnas) { px = 0; }
                        if (px == -1) { px = this._Columnas - 1; }
                        int condicion = this._CheckSelect.Length % this._Columnas;
                        if (condicion == 0)
                        {
                            if (py == this._CheckSelect.Length) { py = 0; }
                            if (py == -this._Columnas) { py = this._CheckSelect.Length - this._Columnas; }
                        }
                        if (condicion != 0)
                        {
                            if ((px + py) >= this._CheckSelect.Length) { py = 0; }
                            else
                            {
                                if (py < 0)
                                {
                                    if (px < condicion)
                                    {
                                        py = (this._CheckSelect.Length - this._Columnas) + 1;
                                    }
                                    if (px == condicion)
                                    {
                                        py = ((this._CheckSelect.Length - this._Columnas) + 1) - this._Columnas;
                                    }
                                }
                            }
                        }
                        pos = px + py;
                    } else
                    {
                        if (tecla.Equals("TAB"))
                        {
                            this._RolesPermisos = 0;
                            this._RolesPermisos = this._RolesPermisos | (1 << pos);
                            script = true;
                        } else { if (tecla.Equals("ENTER")) { estado = false; } }
                    }
                }
                else { estado = false; }
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
            for (int i = 0; i < this._CheckSelect.Length; i++)
            {
                if (valor < this._CheckSelect[i].Length)
                {
                    valor = this._CheckSelect[i].Length;
                }
            }
            return (valor);
        }
        private int CalcularBoxWidth()
        {
            int valor = CalcularMaximo();
            valor += 4; // posicion del checking + seleccion.
            valor += 3; // incrementar espacio.
            valor *= this._Columnas; // incrementar por las columnas.
            valor += 3; //incrementar espacio al principio;
            return (valor);
        }
        private int CalcularBoxHeingth()
        {
            int rt = this._CheckSelect.Length / this._Columnas;
            if ((this._CheckSelect.Length % this._Columnas) != 0)
            {
                rt++;
            }
            rt *= 5;
            rt += 3;
            return (rt);
        }
    }
}
