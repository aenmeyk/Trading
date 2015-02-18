using System.Collections.Generic;

namespace TradeSimulator.Strategies
{
    public class MovementFromYesterdayAllSymbols : MovementFromYesterdayBase
    {
        protected override string Name
        {
            get { return "Movement from yesterday - All symbols"; }
        }

        protected override decimal Spread
        {
            get { return 0.002268016881M; }
        }

        public override IEnumerable<string> Symbols
        {
            get
            {
                return new[]
                {
                    "ACIM",
"BIK",
"CSM",
"CVY",
"CWI",
"DEF",
"DGRE",
"DGRS",
"DGRW",
"DNL",
"DRW",
"DWAS",
"DWX",
"EDIV",
"EDOG",
"EEB",
"EELV",
"EMBB",
"EWEM",
"EWRS",
"EWX",
"FEU",
"FNDA",
"FNDB",
"FNDC",
"FNDE",
"FNDF",
"FNDX",
"GAL",
"GMF",
"GML",
"GXC",
"HGI",
"IBLN",
"IDLV",
"IDOG",
"IHDG",
"INKM",
"JPP",
"KNOW",
"MDD",
"MDYG",
"MDYV",
"NOBL",
"PAF",
"PDP",
"PID",
"PIE",
"PIN",
"PIZ",
"PKW",
"PXMC",
"PXSV",
"QAUS",
"QCAN",
"QDEU",
"QESP",
"QGBR",
"QJPN",
"QKOR",
"QMEX",
"QQQE",
"QTWN",
"RFG",
"RFV",
"RPG",
"RPV",
"RSCO",
"RSP",
"RWO",
"RZG",
"RZV",
"SCHA",
"SCHB",
"SCHC",
"SCHD",
"SCHE",
"SCHF",
"SCHG",
"SCHH",
"SCHM",
"SCHV",
"SCHX",
"SDOG",
"SLYG",
"SLYV",
"SPHB",
"SPLV",
"SYE",
"SYG",
"SYV",
"TOLZ",
"VSPY",
"WDIV",
"WMCR",
"XLG",
"YAO"
                };
            }
        }
    }
}
