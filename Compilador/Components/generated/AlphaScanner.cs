//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.12.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from E:/Compi/Compilador/Compilador/Components\AlphaScanner.g4 by ANTLR 4.12.0

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace generated {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public partial class AlphaScanner : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		USING=1, CLASS=2, VOID=3, NEW=4, IF=5, ELSE=6, FOR=7, WHILE=8, BREAK=9, 
		RETURN=10, READ=11, WRITE=12, CONST=13, ASSIGN=14, INC=15, DEC=16, DOT=17, 
		COMMA=18, SEMICOLON=19, EQUALS=20, NOT_EQUALS=21, LESS_THAN=22, LESS_OR_EQUALS=23, 
		GREATER_THAN=24, GREATER_OR_EQUALS=25, LOGICAL_AND=26, LOGICAL_OR=27, 
		PLUS=28, MULT=29, DIV=30, MOD=31, LEFT_PAREN=32, RIGHT_PAREN=33, LEFT_BRACE=34, 
		RIGHT_BRACE=35, LEFT_BRACKET=36, RIGHT_BRACKET=37, CHAR_CONST=38, STRING_CONST=39, 
		INT_CONST=40, DOUBLE_CONST=41, BOOL_CONST=42, ARRAY=43, IDENTIFIER=44, 
		MINUSEXP=45, WS=46, COMMENT=47, LINE_COMMENT=48;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"USING", "CLASS", "VOID", "NEW", "IF", "ELSE", "FOR", "WHILE", "BREAK", 
		"RETURN", "READ", "WRITE", "CONST", "ASSIGN", "INC", "DEC", "DOT", "COMMA", 
		"SEMICOLON", "EQUALS", "NOT_EQUALS", "LESS_THAN", "LESS_OR_EQUALS", "GREATER_THAN", 
		"GREATER_OR_EQUALS", "LOGICAL_AND", "LOGICAL_OR", "PLUS", "MULT", "DIV", 
		"MOD", "LEFT_PAREN", "RIGHT_PAREN", "LEFT_BRACE", "RIGHT_BRACE", "LEFT_BRACKET", 
		"RIGHT_BRACKET", "CHAR_CONST", "STRING_CONST", "INT_CONST", "DOUBLE_CONST", 
		"BOOL_CONST", "ARRAY", "IDENTIFIER", "MINUSEXP", "TRUE", "FALSE", "DIGIT", 
		"LETTER", "QUOMARK", "MINIQUOMARK", "SPECIAL", "MINUS", "WS", "COMMENT", 
		"LINE_COMMENT"
	};


	                 public override void NotifyListeners(LexerNoViableAltException e){
	                 this.ErrorListenerDispatch.SyntaxError(this.ErrorOutput, (IRecognizer) this, 0, TokenStartLine, this.TokenStartColumn,"token invalido: '" + this.GetErrorDisplay(this.EmitEOF().InputStream.GetText(Interval.Of(this.TokenStartCharIndex,this.InputStream.Index)))  + "'", (RecognitionException) e);
	                }
	 

	public AlphaScanner(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public AlphaScanner(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'using'", "'class'", "'void'", "'new'", "'if'", "'else'", "'for'", 
		"'while'", "'break'", "'return'", "'read'", "'write'", "'const'", "'='", 
		"'++'", "'--'", "'.'", "','", "';'", "'=='", "'!='", "'<'", "'<='", "'>'", 
		"'>='", "'&&'", "'||'", "'+'", "'*'", "'/'", "'%'", "'('", "')'", "'{'", 
		"'}'", "'['", "']'", null, null, null, null, null, "'array'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "USING", "CLASS", "VOID", "NEW", "IF", "ELSE", "FOR", "WHILE", "BREAK", 
		"RETURN", "READ", "WRITE", "CONST", "ASSIGN", "INC", "DEC", "DOT", "COMMA", 
		"SEMICOLON", "EQUALS", "NOT_EQUALS", "LESS_THAN", "LESS_OR_EQUALS", "GREATER_THAN", 
		"GREATER_OR_EQUALS", "LOGICAL_AND", "LOGICAL_OR", "PLUS", "MULT", "DIV", 
		"MOD", "LEFT_PAREN", "RIGHT_PAREN", "LEFT_BRACE", "RIGHT_BRACE", "LEFT_BRACKET", 
		"RIGHT_BRACKET", "CHAR_CONST", "STRING_CONST", "INT_CONST", "DOUBLE_CONST", 
		"BOOL_CONST", "ARRAY", "IDENTIFIER", "MINUSEXP", "WS", "COMMENT", "LINE_COMMENT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "AlphaScanner.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static AlphaScanner() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,48,355,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,2,48,7,48,2,49,
		7,49,2,50,7,50,2,51,7,51,2,52,7,52,2,53,7,53,2,54,7,54,2,55,7,55,1,0,1,
		0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,2,1,2,1,2,1,2,1,3,1,3,
		1,3,1,3,1,4,1,4,1,4,1,5,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,6,1,7,1,7,1,7,1,
		7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,1,9,1,9,1,10,1,10,
		1,10,1,10,1,10,1,11,1,11,1,11,1,11,1,11,1,11,1,12,1,12,1,12,1,12,1,12,
		1,12,1,13,1,13,1,14,1,14,1,14,1,15,1,15,1,15,1,16,1,16,1,17,1,17,1,18,
		1,18,1,19,1,19,1,19,1,20,1,20,1,20,1,21,1,21,1,22,1,22,1,22,1,23,1,23,
		1,24,1,24,1,24,1,25,1,25,1,25,1,26,1,26,1,26,1,27,1,27,1,28,1,28,1,29,
		1,29,1,30,1,30,1,31,1,31,1,32,1,32,1,33,1,33,1,34,1,34,1,35,1,35,1,36,
		1,36,1,37,1,37,1,37,1,37,1,37,3,37,244,8,37,1,37,1,37,1,38,1,38,5,38,250,
		8,38,10,38,12,38,253,9,38,1,38,1,38,1,39,4,39,258,8,39,11,39,12,39,259,
		1,40,4,40,263,8,40,11,40,12,40,264,1,40,1,40,4,40,269,8,40,11,40,12,40,
		270,1,41,1,41,3,41,275,8,41,1,42,1,42,1,42,1,42,1,42,1,42,1,43,1,43,1,
		43,5,43,286,8,43,10,43,12,43,289,9,43,1,44,1,44,1,45,1,45,1,45,1,45,1,
		45,1,46,1,46,1,46,1,46,1,46,1,46,1,47,1,47,1,48,1,48,1,49,1,49,1,50,1,
		50,1,51,1,51,1,52,1,52,1,53,4,53,317,8,53,11,53,12,53,318,1,53,1,53,1,
		54,1,54,1,54,1,54,5,54,327,8,54,10,54,12,54,330,9,54,1,54,5,54,333,8,54,
		10,54,12,54,336,9,54,1,54,1,54,1,54,1,54,1,54,1,55,1,55,1,55,1,55,5,55,
		347,8,55,10,55,12,55,350,9,55,1,55,1,55,1,55,1,55,3,251,328,348,0,56,1,
		1,3,2,5,3,7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,
		15,31,16,33,17,35,18,37,19,39,20,41,21,43,22,45,23,47,24,49,25,51,26,53,
		27,55,28,57,29,59,30,61,31,63,32,65,33,67,34,69,35,71,36,73,37,75,38,77,
		39,79,40,81,41,83,42,85,43,87,44,89,45,91,0,93,0,95,0,97,0,99,0,101,0,
		103,0,105,0,107,46,109,47,111,48,1,0,5,1,0,48,57,3,0,65,90,95,95,97,122,
		338,0,33,47,58,64,91,96,123,126,161,169,171,172,174,177,180,180,182,184,
		187,187,191,191,215,215,247,247,706,709,722,735,741,747,749,749,751,767,
		885,885,894,894,900,901,903,903,1014,1014,1154,1154,1370,1375,1417,1418,
		1421,1423,1470,1470,1472,1472,1475,1475,1478,1478,1523,1524,1542,1551,
		1563,1563,1565,1567,1642,1645,1748,1748,1758,1758,1769,1769,1789,1790,
		1792,1805,2038,2041,2046,2047,2096,2110,2142,2142,2184,2184,2404,2405,
		2416,2416,2546,2547,2554,2555,2557,2557,2678,2678,2800,2801,2928,2928,
		3059,3066,3191,3191,3199,3199,3204,3204,3407,3407,3449,3449,3572,3572,
		3647,3647,3663,3663,3674,3675,3841,3863,3866,3871,3892,3892,3894,3894,
		3896,3896,3898,3901,3973,3973,4030,4037,4039,4044,4046,4058,4170,4175,
		4254,4255,4347,4347,4960,4968,5008,5017,5120,5120,5741,5742,5787,5788,
		5867,5869,5941,5942,6100,6102,6104,6107,6144,6154,6464,6464,6468,6469,
		6622,6655,6686,6687,6816,6822,6824,6829,7002,7018,7028,7038,7164,7167,
		7227,7231,7294,7295,7360,7367,7379,7379,8125,8125,8127,8129,8141,8143,
		8157,8159,8173,8175,8189,8190,8208,8231,8240,8286,8314,8318,8330,8334,
		8352,8384,8448,8449,8451,8454,8456,8457,8468,8468,8470,8472,8478,8483,
		8485,8485,8487,8487,8489,8489,8494,8494,8506,8507,8512,8516,8522,8525,
		8527,8527,8586,8587,8592,9254,9280,9290,9372,9449,9472,10101,10132,11123,
		11126,11157,11159,11263,11493,11498,11513,11516,11518,11519,11632,11632,
		11776,11822,11824,11869,11904,11929,11931,12019,12032,12245,12272,12283,
		12289,12292,12296,12320,12336,12336,12342,12343,12349,12351,12443,12444,
		12448,12448,12539,12539,12688,12689,12694,12703,12736,12771,12800,12830,
		12842,12871,12880,12880,12896,12927,12938,12976,12992,13311,19904,19967,
		42128,42182,42238,42239,42509,42511,42611,42611,42622,42622,42738,42743,
		42752,42774,42784,42785,42889,42890,43048,43051,43062,43065,43124,43127,
		43214,43215,43256,43258,43260,43260,43310,43311,43359,43359,43457,43469,
		43486,43487,43612,43615,43639,43641,43742,43743,43760,43761,43867,43867,
		43882,43883,44011,44011,64297,64297,64434,64450,64830,64847,64975,64975,
		65020,65023,65040,65049,65072,65106,65108,65126,65128,65131,65281,65295,
		65306,65312,65339,65344,65371,65381,65504,65510,65512,65518,65532,65533,
		65792,65794,65847,65855,65913,65929,65932,65934,65936,65948,65952,65952,
		66000,66044,66463,66463,66512,66512,66927,66927,67671,67671,67703,67704,
		67871,67871,67903,67903,68176,68184,68223,68223,68296,68296,68336,68342,
		68409,68415,68505,68508,69293,69293,69461,69465,69510,69513,69703,69709,
		69819,69820,69822,69825,69952,69955,70004,70005,70085,70088,70093,70093,
		70107,70107,70109,70111,70200,70205,70313,70313,70731,70735,70746,70747,
		70749,70749,70854,70854,71105,71127,71233,71235,71264,71276,71353,71353,
		71484,71487,71739,71739,72004,72006,72162,72162,72255,72262,72346,72348,
		72350,72354,72769,72773,72816,72817,73463,73464,73685,73713,73727,73727,
		74864,74868,77809,77810,92782,92783,92917,92917,92983,92991,92996,92997,
		93847,93850,94178,94178,113820,113820,113823,113823,118608,118723,118784,
		119029,119040,119078,119081,119140,119146,119148,119171,119172,119180,
		119209,119214,119274,119296,119361,119365,119365,119552,119638,120513,
		120513,120539,120539,120571,120571,120597,120597,120629,120629,120655,
		120655,120687,120687,120713,120713,120745,120745,120771,120771,120832,
		121343,121399,121402,121453,121460,121462,121475,121477,121483,123215,
		123215,123647,123647,125278,125279,126124,126124,126128,126128,126254,
		126254,126704,126705,126976,127019,127024,127123,127136,127150,127153,
		127167,127169,127183,127185,127221,127245,127405,127462,127490,127504,
		127547,127552,127560,127568,127569,127584,127589,127744,128727,128733,
		128748,128752,128764,128768,128883,128896,128984,128992,129003,129008,
		129008,129024,129035,129040,129095,129104,129113,129120,129159,129168,
		129197,129200,129201,129280,129619,129632,129645,129648,129652,129656,
		129660,129664,129670,129680,129708,129712,129722,129728,129733,129744,
		129753,129760,129767,129776,129782,129792,129938,129940,129994,3,0,9,10,
		13,13,32,32,2,0,10,10,13,13,360,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,
		7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,
		0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,
		29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,
		0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,
		0,51,1,0,0,0,0,53,1,0,0,0,0,55,1,0,0,0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,
		1,0,0,0,0,63,1,0,0,0,0,65,1,0,0,0,0,67,1,0,0,0,0,69,1,0,0,0,0,71,1,0,0,
		0,0,73,1,0,0,0,0,75,1,0,0,0,0,77,1,0,0,0,0,79,1,0,0,0,0,81,1,0,0,0,0,83,
		1,0,0,0,0,85,1,0,0,0,0,87,1,0,0,0,0,89,1,0,0,0,0,107,1,0,0,0,0,109,1,0,
		0,0,0,111,1,0,0,0,1,113,1,0,0,0,3,119,1,0,0,0,5,125,1,0,0,0,7,130,1,0,
		0,0,9,134,1,0,0,0,11,137,1,0,0,0,13,142,1,0,0,0,15,146,1,0,0,0,17,152,
		1,0,0,0,19,158,1,0,0,0,21,165,1,0,0,0,23,170,1,0,0,0,25,176,1,0,0,0,27,
		182,1,0,0,0,29,184,1,0,0,0,31,187,1,0,0,0,33,190,1,0,0,0,35,192,1,0,0,
		0,37,194,1,0,0,0,39,196,1,0,0,0,41,199,1,0,0,0,43,202,1,0,0,0,45,204,1,
		0,0,0,47,207,1,0,0,0,49,209,1,0,0,0,51,212,1,0,0,0,53,215,1,0,0,0,55,218,
		1,0,0,0,57,220,1,0,0,0,59,222,1,0,0,0,61,224,1,0,0,0,63,226,1,0,0,0,65,
		228,1,0,0,0,67,230,1,0,0,0,69,232,1,0,0,0,71,234,1,0,0,0,73,236,1,0,0,
		0,75,238,1,0,0,0,77,247,1,0,0,0,79,257,1,0,0,0,81,262,1,0,0,0,83,274,1,
		0,0,0,85,276,1,0,0,0,87,282,1,0,0,0,89,290,1,0,0,0,91,292,1,0,0,0,93,297,
		1,0,0,0,95,303,1,0,0,0,97,305,1,0,0,0,99,307,1,0,0,0,101,309,1,0,0,0,103,
		311,1,0,0,0,105,313,1,0,0,0,107,316,1,0,0,0,109,322,1,0,0,0,111,342,1,
		0,0,0,113,114,5,117,0,0,114,115,5,115,0,0,115,116,5,105,0,0,116,117,5,
		110,0,0,117,118,5,103,0,0,118,2,1,0,0,0,119,120,5,99,0,0,120,121,5,108,
		0,0,121,122,5,97,0,0,122,123,5,115,0,0,123,124,5,115,0,0,124,4,1,0,0,0,
		125,126,5,118,0,0,126,127,5,111,0,0,127,128,5,105,0,0,128,129,5,100,0,
		0,129,6,1,0,0,0,130,131,5,110,0,0,131,132,5,101,0,0,132,133,5,119,0,0,
		133,8,1,0,0,0,134,135,5,105,0,0,135,136,5,102,0,0,136,10,1,0,0,0,137,138,
		5,101,0,0,138,139,5,108,0,0,139,140,5,115,0,0,140,141,5,101,0,0,141,12,
		1,0,0,0,142,143,5,102,0,0,143,144,5,111,0,0,144,145,5,114,0,0,145,14,1,
		0,0,0,146,147,5,119,0,0,147,148,5,104,0,0,148,149,5,105,0,0,149,150,5,
		108,0,0,150,151,5,101,0,0,151,16,1,0,0,0,152,153,5,98,0,0,153,154,5,114,
		0,0,154,155,5,101,0,0,155,156,5,97,0,0,156,157,5,107,0,0,157,18,1,0,0,
		0,158,159,5,114,0,0,159,160,5,101,0,0,160,161,5,116,0,0,161,162,5,117,
		0,0,162,163,5,114,0,0,163,164,5,110,0,0,164,20,1,0,0,0,165,166,5,114,0,
		0,166,167,5,101,0,0,167,168,5,97,0,0,168,169,5,100,0,0,169,22,1,0,0,0,
		170,171,5,119,0,0,171,172,5,114,0,0,172,173,5,105,0,0,173,174,5,116,0,
		0,174,175,5,101,0,0,175,24,1,0,0,0,176,177,5,99,0,0,177,178,5,111,0,0,
		178,179,5,110,0,0,179,180,5,115,0,0,180,181,5,116,0,0,181,26,1,0,0,0,182,
		183,5,61,0,0,183,28,1,0,0,0,184,185,5,43,0,0,185,186,5,43,0,0,186,30,1,
		0,0,0,187,188,5,45,0,0,188,189,5,45,0,0,189,32,1,0,0,0,190,191,5,46,0,
		0,191,34,1,0,0,0,192,193,5,44,0,0,193,36,1,0,0,0,194,195,5,59,0,0,195,
		38,1,0,0,0,196,197,5,61,0,0,197,198,5,61,0,0,198,40,1,0,0,0,199,200,5,
		33,0,0,200,201,5,61,0,0,201,42,1,0,0,0,202,203,5,60,0,0,203,44,1,0,0,0,
		204,205,5,60,0,0,205,206,5,61,0,0,206,46,1,0,0,0,207,208,5,62,0,0,208,
		48,1,0,0,0,209,210,5,62,0,0,210,211,5,61,0,0,211,50,1,0,0,0,212,213,5,
		38,0,0,213,214,5,38,0,0,214,52,1,0,0,0,215,216,5,124,0,0,216,217,5,124,
		0,0,217,54,1,0,0,0,218,219,5,43,0,0,219,56,1,0,0,0,220,221,5,42,0,0,221,
		58,1,0,0,0,222,223,5,47,0,0,223,60,1,0,0,0,224,225,5,37,0,0,225,62,1,0,
		0,0,226,227,5,40,0,0,227,64,1,0,0,0,228,229,5,41,0,0,229,66,1,0,0,0,230,
		231,5,123,0,0,231,68,1,0,0,0,232,233,5,125,0,0,233,70,1,0,0,0,234,235,
		5,91,0,0,235,72,1,0,0,0,236,237,5,93,0,0,237,74,1,0,0,0,238,243,3,101,
		50,0,239,244,3,97,48,0,240,244,3,95,47,0,241,244,3,107,53,0,242,244,3,
		103,51,0,243,239,1,0,0,0,243,240,1,0,0,0,243,241,1,0,0,0,243,242,1,0,0,
		0,244,245,1,0,0,0,245,246,3,101,50,0,246,76,1,0,0,0,247,251,3,99,49,0,
		248,250,9,0,0,0,249,248,1,0,0,0,250,253,1,0,0,0,251,252,1,0,0,0,251,249,
		1,0,0,0,252,254,1,0,0,0,253,251,1,0,0,0,254,255,3,99,49,0,255,78,1,0,0,
		0,256,258,3,95,47,0,257,256,1,0,0,0,258,259,1,0,0,0,259,257,1,0,0,0,259,
		260,1,0,0,0,260,80,1,0,0,0,261,263,3,95,47,0,262,261,1,0,0,0,263,264,1,
		0,0,0,264,262,1,0,0,0,264,265,1,0,0,0,265,266,1,0,0,0,266,268,5,46,0,0,
		267,269,3,95,47,0,268,267,1,0,0,0,269,270,1,0,0,0,270,268,1,0,0,0,270,
		271,1,0,0,0,271,82,1,0,0,0,272,275,3,91,45,0,273,275,3,93,46,0,274,272,
		1,0,0,0,274,273,1,0,0,0,275,84,1,0,0,0,276,277,5,97,0,0,277,278,5,114,
		0,0,278,279,5,114,0,0,279,280,5,97,0,0,280,281,5,121,0,0,281,86,1,0,0,
		0,282,287,3,97,48,0,283,286,3,97,48,0,284,286,3,95,47,0,285,283,1,0,0,
		0,285,284,1,0,0,0,286,289,1,0,0,0,287,285,1,0,0,0,287,288,1,0,0,0,288,
		88,1,0,0,0,289,287,1,0,0,0,290,291,3,105,52,0,291,90,1,0,0,0,292,293,5,
		116,0,0,293,294,5,114,0,0,294,295,5,117,0,0,295,296,5,101,0,0,296,92,1,
		0,0,0,297,298,5,102,0,0,298,299,5,97,0,0,299,300,5,108,0,0,300,301,5,115,
		0,0,301,302,5,101,0,0,302,94,1,0,0,0,303,304,7,0,0,0,304,96,1,0,0,0,305,
		306,7,1,0,0,306,98,1,0,0,0,307,308,5,34,0,0,308,100,1,0,0,0,309,310,5,
		39,0,0,310,102,1,0,0,0,311,312,7,2,0,0,312,104,1,0,0,0,313,314,5,45,0,
		0,314,106,1,0,0,0,315,317,7,3,0,0,316,315,1,0,0,0,317,318,1,0,0,0,318,
		316,1,0,0,0,318,319,1,0,0,0,319,320,1,0,0,0,320,321,6,53,0,0,321,108,1,
		0,0,0,322,323,5,47,0,0,323,324,5,42,0,0,324,328,1,0,0,0,325,327,9,0,0,
		0,326,325,1,0,0,0,327,330,1,0,0,0,328,329,1,0,0,0,328,326,1,0,0,0,329,
		334,1,0,0,0,330,328,1,0,0,0,331,333,8,4,0,0,332,331,1,0,0,0,333,336,1,
		0,0,0,334,332,1,0,0,0,334,335,1,0,0,0,335,337,1,0,0,0,336,334,1,0,0,0,
		337,338,5,42,0,0,338,339,5,47,0,0,339,340,1,0,0,0,340,341,6,54,0,0,341,
		110,1,0,0,0,342,343,5,47,0,0,343,344,5,47,0,0,344,348,1,0,0,0,345,347,
		9,0,0,0,346,345,1,0,0,0,347,350,1,0,0,0,348,349,1,0,0,0,348,346,1,0,0,
		0,349,351,1,0,0,0,350,348,1,0,0,0,351,352,7,4,0,0,352,353,1,0,0,0,353,
		354,6,55,0,0,354,112,1,0,0,0,13,0,243,251,259,264,270,274,285,287,318,
		328,334,348,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace generated
