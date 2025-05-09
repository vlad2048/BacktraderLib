# Totals

| Entity | Count |
|-|-
| Trading212 Symbols | 14781
| TwelveData Symbols | 154874
| Mics | 2756



<br><br>

# Trading212 Symbols
```
Total               14781
Type=STOCK          10876
Type=ETF            3890
Type=WARRANT        15
```


<br><br>

# TwelveData Symbols
```
Total               154874
FigiCode == ""      20320
```



<br><br>

# TwelveData CfiCode

```
Total Count:	154897
======================


CfiCode[..2]		Type
-------------------------------------------
ES					Common Stock
ED					Depositary Receipt
EU					Unit
EL					Limited Partnership
EP					Preferred Stock
EY					Structured Product
CE					ETF
CI					Mutual Fund | Closed-end Fund


***********
* CfiCode *
***********

	""			2505
	****************

	E			152328		Equities
	********************************

		ES			150567			Shares i.e. Common / Ordinary
		=========================================================

			Voting Right
			------------
			ESV___		147004				Voting
			ESN___		1692				Non-Voting
			ESR___		1151				Restricted
			ESE___		720					Enhanced voting

			Ownership
			---------
			ES_U__		111857				Free
			ES_T__		38710				Restrictions

			Payment Status
			--------------
			ES__F_		149403				Fully Paid
			ES__P_		899					Partly Paid
			ES__O_		265					Nil Paid

			Form
			----
			ES___R		150073				Registered
			ES___M		440					Others (Misc.)
			ES___B		35					Bearer
			ES___N		19					Bearer/Registered


		ED			332				Depository receipts on equities
		===========================================================

		EU			182				Units (from Unit trusts, Mutual funds, OPCVM or OICVM)
		==================================================================================

		EL			56				Limited partnership units
		=====================================================

		EP			1183			Preferred shares
		============================================

		EY			8				Structured instruments (participation)
		==================================================================

	C			64			Collective Investment Vehicles
	******************************************************
```



<br><br>

# Trading212.Exchange -> Mic mapping

| Trading212.Exchange           | Count     | Mic                   | Name                                  | TwelveData.Exchange
|-|-|-|-|-
| Gettex                        | 521       | XMUN                  | BOERSE MUENCHEN                       | Munich
| Bolsa de Madrid               | 171       | (BMEX) > XMAD         | BOLSA DE MADRID                       | BME
| Borsa Italiana                | 330       | XMIL                  | BORSA ITALIANA GLOBAL EQUITY MARKET   | MTA
| Deutsche Börse Xetra          | 1234      | XETR                  | XETRA                                 | XETR
| Euronext Amsterdam            | 237       | XAMS                  | EURONEXT - EURONEXT AMSTERDAM         | Euronext
| Euronext Brussels             | 115       | XBRU                  | EURONEXT - EURONEXT BRUSSELS          | Euronext
| Euronext Lisbon               | 22        | XLIS                  | EURONEXT - EURONEXT LISBON            | Euronext
| Euronext Paris                | 426       | XPAR                  | EURONEXT - EURONEXT PARIS             | Euronext
| London Stock Exchange         | 3223      | XLON                  | LONDON STOCK EXCHANGE                 | LSE
| London Stock Exchange AIM     | 524       | (XLON) > .AIMX        | LONDON STOCK EXCHANGE - AIM MTF       | LSE
| NASDAQ                        | 3268      | XNAS > XNCM,XNGS,XNMS | NASDAQ - ALL MARKETS                  | NASDAQ
| NYSE                          | 2120      | XNYS > ARCX,XASE      | NEW YORK STOCK EXCHANGE, INC.         | NYSE
| SIX Swiss Exchange            | 413       | XSWX                  | SIX SWISS EXCHANGE                    | SIX
| Toronto Stock Exchange        | 544       | XTSE                  | TORONTO STOCK EXCHANGE                | TSX
| Wiener Börse                  | 64        | XWBO                  | WIENER BOERSE AG                      | VSE
| OTC Markets                   | 1569      | ?

XNAS                NASDAQ - ALL MARKETS
    XNCM            NASDAQ CAPITAL MARKET
    XNGS            NASDAQ/NGS (GLOBAL SELECT MARKET)
    XNMS            NASDAQ/NMS (GLOBAL MARKET)

XNYS                NEW YORK STOCK EXCHANGE, INC.
    ARCX            NYSE ARCA
    XASE            NYSE AMERICAN LLC

