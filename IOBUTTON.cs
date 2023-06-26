using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;
using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IOBUTTON : IOdata , FuncIOData
    {
        private string _Titulo;
        private bool _RolesPermisos;
        private color[] _BackCorral;
        private color[] _ForeCorral;
        private color[] _Backtitulo;
        private color[] _Foretitulo;
        private TypeLine _Line;        
        private int _PosX;
        private int _PosY;

        public IOBUTTON(string titulo, color[] backCorral, color[] foreCorral, color[] backtitulo, color[] foretitulo, TypeLine line, int posX, int posY)
        {
            this._Titulo = titulo;
            this._RolesPermisos = false;
            this._BackCorral = backCorral;
            this._ForeCorral = foreCorral;
            this._Backtitulo = backtitulo;
            this._Foretitulo = foretitulo;
            this._Line = line;
            this._PosX = posX;
            this._PosY = posY;
        }

        public void Display(color back, color fore)
        {
            bool estado = true, script = false;
            int condicion_color = (int)Enum.Parse(typeof(TypeStateIO), this._StateEvent.ToString());
            color bcorral = this._BackCorral[condicion_color];
            color fcorral = this._ForeCorral[condicion_color];
            IN keydata = new IN();

            switch(this._StateEvent)
            {
                case (TypeStateIO._INACTIVATED): condicion_color = 0; break;
                case (TypeStateIO._ACTIVATED): condicion_color = 1; break;
            }

            // imprimir caja
            DRAW.TablaLine(this._Line, bcorral, fcorral, new int[] { (this._Titulo.Length + 8) }, new int[] { 4 }, this._PosX, this._PosY);

            if (this._StateEvent == TypeStateIO._ACTIVATED)
            {
                keydata.SetCondIN(INCond._TAB);
                keydata.SetCondIN(INCond._ENTER);
            }

            while (estado)
            {               
                
                color btitulo = this._Backtitulo[condicion_color];
                color ftitulo = this._Foretitulo[condicion_color];
                // titulo
                SelectorMedio(btitulo, ftitulo, this._Titulo, this._Titulo.Length + 5, (this._PosX + 1), (this._PosY + 1));
                if (script)
                {
                    System.Threading.Thread.Sleep(600);
                    estado = false;
                }
                if ((this._StateEvent == TypeStateIO._ACTIVATED) && (!(script)))
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = keydata.InputMode();
                    if (tecla.Equals("TAB")) { estado = false; }
                    else
                    {
                        if (tecla.Equals("ENTER"))
                        {
                            script = true;
                            this._RolesPermisos = true;
                            condicion_color++;
                        }
                    }
                } else { estado = false; }                
            }
        }
        public object GetDataInfo() => this._RolesPermisos;
        public void SetDataInfo(object dataInfo) => this._RolesPermisos = Convert.ToBoolean(dataInfo);
        void FuncIOData.SetTypeDataIN(TypeDataIN cond)
        {
            throw new NotImplementedException();
        }
    }
}
