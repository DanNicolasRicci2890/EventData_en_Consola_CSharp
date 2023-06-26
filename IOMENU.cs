using PCD_ColorFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using PCD_INOUT_INFO;

namespace PCD_EVENT_DATA
{
    public class IOMENU : IOdata, FuncIOData
    {
        private string _Titulo;
        private string[] _Selection;
        private int _Columnas;
        private color _BackCorral;
        private color _ForeCorral;
        private color _BackTitulo;
        private color _ForeTitulo;
        private color[] _BackBoxLine;
        private color[] _ForeBoxLine;
        private color[] _BackSelect;
        private color[] _ForeSelect;
        private int _Roles;
        private int _Value;
        private int _PosX;
        private int _PosY;

        public IOMENU(string titulo, string[] selection, int columnas, color backcorral, color forecorral, color backtitulo, color foretitulo, color[] backboxline, color[] foreboxline, color[] backselect, color[] foreselect, int posx, int posy)
        {
            string fret = titulo.PadLeft(titulo.Length + 4);
            this._Titulo = fret.PadRight(fret.Length + 4);
            this._Selection = FormadorSelect(selection);
            this._Columnas = columnas;
            this._BackCorral = backcorral;
            this._ForeCorral = forecorral;
            this._BackTitulo = backtitulo;
            this._ForeTitulo = foretitulo;
            this._BackBoxLine = backboxline;
            this._ForeBoxLine = foreboxline;
            this._BackSelect = backselect;
            this._ForeSelect = foreselect;
            this._Roles = 0;
            this._Value = -1;
            this._PosX = posx;
            this._PosY = posy;
        }

        public void Display(color back, color fore)
        {
            bool estado = true;
            int width_corral = ((Capacitador(ref this._Selection)) * this._Columnas) + ((this._Columnas + 1) * 15);
            int heinght_corral = this._Selection.Length / this._Columnas;
            color BBL = color.none, FBL = color.none, BS = color.none, FS = color.none;
            int pos = 0, color_posicion = 0, width = Capacitador(ref this._Selection);
            int cont_columnas = 1, posicionX = 14, posicionY = 5;
            bool script = false;
            IN key_data = new IN();

            key_data.SetCondIN(INCond._ARROWS);
            key_data.SetCondIN(INCond._ENTER);
            if ((this._Selection.Length % this._Columnas) != 0)
            {
                heinght_corral++;
            }
            heinght_corral = (heinght_corral * 5) + 5; 
            DRAW.TablaLine(TypeLine._DOUBLE, this._BackCorral, this._ForeCorral, new int[] { width_corral }, new int[] { heinght_corral }, this._PosX, this._PosY);
            Selector(this._BackTitulo, this._ForeTitulo, this._Titulo, this._Titulo.Length, ((width_corral - (this._Titulo.Length)) / 2) + this._PosX, this._PosY + 1);
            while(estado)
            {
                for(int i = 0; i < this._Selection.Length; i++)
                {
                    if (((this._Roles >> i) & 1) == 1)
                    {
                        // rol obtenido.
                        if (pos == i)
                        {
                            // rol obtenido elegido
                            switch (script)
                            {
                                // rol obtenido elegido no aceptado 
                                case false: color_posicion = 3; break;

                                // rol obtenido elegido aceptado
                                case true: color_posicion = 4; break;
                            }
                        }
                        else
                        {
                            // rol obtenido no elegido
                            color_posicion = 2;
                        }
                    } else
                    {
                        // rol no obtenido.
                        if (pos == i)
                        {
                            // rol no obtenido elegido
                            color_posicion = 1;
                        } else
                        {
                            // rol no obtenido no elegido
                            color_posicion = 0;
                        }
                    }

                    BBL = this._BackBoxLine[color_posicion];
                    FBL = this._ForeBoxLine[color_posicion];
                    BS = this._BackSelect[color_posicion];
                    FS = this._ForeSelect[color_posicion];                    
                    DRAW.CuadradoLineDouble(BBL, FBL, width + 2, 3, this._PosX + posicionX, this._PosY + posicionY);
                    Selector(BS, FS, this._Selection[i], width, this._PosX + posicionX + 1, this._PosY + posicionY + 1);                    
                    if (cont_columnas == this._Columnas)
                    {
                        cont_columnas = 1;
                        posicionX = 14;
                        posicionY += 5;
                    }
                    else
                    {
                        posicionX += (width + 15);
                        cont_columnas++;
                    }
                }               
                if (!script)
                {
                    OUT.PrintLine("", fore, back, 0, 0);
                    string tecla = key_data.InputMode();
                    if ((!(tecla.Equals("ENTER"))) && (!(tecla.Equals(""))))
                    {
                        if (tecla.Equals("RIGHTARROW"))
                        {
                            pos++;
                            if (pos == this._Selection.Length) { pos = 0; }
                        } else
                        {
                            if (tecla.Equals("LEFTARROW"))
                            {
                                pos--;
                                if (pos == -1) { pos = this._Selection.Length - 1; }
                            } else
                            {
                                if (tecla.Equals("DOWNARROW"))
                                {
                                    pos += this._Columnas;
                                    if (pos >= this._Selection.Length) 
                                    {
                                        while(pos > 0)
                                        {
                                            pos -= this._Columnas;
                                        }   
                                        if (pos < 0) { pos += this._Columnas; }
                                    }
                                } else
                                {
                                    if (tecla.Equals("UPARROW"))
                                    {
                                        pos -= this._Columnas;

                                        if (pos < 0)
                                        {
                                            while(pos <= this._Selection.Length - 1)
                                            {
                                                pos += this._Columnas;
                                            }
                                            pos -= this._Columnas;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else 
                    { 
                        if (tecla.Equals("ENTER")) 
                        { 
                            script = true; 
                        } 
                    }                    
                } else
                {
                    if (((this._Roles >> pos) & 1) == 1)
                    {
                        estado = false;
                        this._Value = pos;
                        System.Threading.Thread.Sleep(1200);

                    } else { script = false; }
                }

                posicionX = 14;
                posicionY = 5;
                cont_columnas = 1;
            }
        }
        public object GetDataInfo() => this._Value;
        public void SetDataInfo(object dataInfo) => this._Roles = Convert.ToInt32(dataInfo);
        void FuncIOData.SetTypeDataIN(TypeDataIN cond)
        {
            throw new NotImplementedException();
        }
        private string[] FormadorSelect(string[] listado)
        {
            string[] resultado = new string[listado.Length];
            int tyr = Capacitador(ref listado) + 4;

            
            for(int i = 0; i < listado.Length; i++)
            {
                int hint = (tyr - listado[i].Length) / 2;
                string hstr = listado[i].PadLeft(listado[i].Length + hint);
                hint = tyr - hstr.Length;
                resultado[i] = hstr.PadRight(hstr.Length + hint);               
            }
            return (resultado);
        }
        private int Capacitador(ref string[] listado)
        {
            int tyr = 0;
            for (int i = 0; i < listado.Length; i++)
            {
                if (tyr < listado[i].Length)
                {
                    tyr = listado[i].Length;
                }
            }
            return (tyr);
        }
    }
}
