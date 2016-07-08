using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ZedGraph;

namespace _3DGimbal
{
    class CurveProperty
    {
        public CurveProperty(String curveName, Color curveColor,SymbolType curveSymbolType)
        {
            this.curveName = curveName;
            this.curveColor = curveColor;
            this.curveSymbolType = curveSymbolType;
        }
        private String curveName;
        public String CurveName
        {
            get { return curveName; }
            set { curveName = value; }
        }
        private Color curveColor;
        public Color CurveColor
        {
            get { return curveColor; }
            set { curveColor = value; }
        }
        private SymbolType curveSymbolType;
        public SymbolType CurveSymbolType
        {
            get { return curveSymbolType; }
            set { curveSymbolType = value; }
        }

    }
}
