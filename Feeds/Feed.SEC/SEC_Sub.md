# Sub

For a given Name, these fields are always the same:
- Cik
- Sic
- Former
- Changed
- Afs
- Prevrpt   (not true!)

Whereas these can change:
- Adsh
- Wksi
- Form
- Period
- Fy
- Fp
- Filed
- Accepted
- Detail
- Instance


## - Name
- 20916 distinct ones
- min length: 2
- max length (allowed): 150
- max length (effective): 95
- characters used: " !#&'(),-./0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ\"
- the first letter can be any of: "'0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
- all names contain at least one upper case letter
 
to make them file safe we substitute:
  - '/' -> '+'
  - '\\' -> '='
  - ':' -> '@'

furthermore, file and directory names cannot end in '.', so we also substitute:
  - '.' -> '~'


## - Sic
- not always specified (null)
- when specified, always found in the table defined in SicCodeExt.cs

||||
|-|-|-
|2834	| x26688	| Pharmaceutical Preparations
|6798	| x16283	| Real Estate Investment Trusts
|6770	| x14612	| Blank Checks
|...
|4412	| x1212		| Deep Sea Foreign Transportation of Freight
|null	| x1171
|4700	| x1138		| Transportation Services


## - Afs
|||
|-|-
|LargeAccelerated				| x114431
|NonAccelerated					| x110873
|SmallerReportingFiler			| x95073
|Accelerated					| x65317
|NotAssigned					| x1353
|SmallerReportingAccelerated	| x211


## - Wksi
|||
|-|-
|False	| x347420
|True	| x39838


## - Fye
Fiscal Year End Date, rounded to nearest month-end (mmdd). ie: 1231
- can be null
- in some rare cases, it is not the last day of the month


## - Form
|||
|-|-
|10-Q		| x261017
|10-K		| x83289
|10-Q/A		| x11273
|20-F		| x7100
|10-K/A		| x5173
|S-1/A		| x4195
|6-K		| x3264
|S-1		| x3176
|POS AM		| x1698
|8-K		| x1335
|S-4/A		| x1322
|40-F		| x1047
|S-4		| x811
|20-F/A		| x801
|10-KT		| x285
|6-K/A		| x243
|F-1/A		| x217
|F-1		| x200
|8-K/A		| x160
|40-F/A		| x98
|10-QT		| x76
|10-12G/A	| x73
|S-11/A		| x70
|424B3		| x56
|F-4/A		| x45
|10-KT/A	| x35
|10-12G		| x35
|S-11		| x31
|F-4		| x30
|POS EX		| x14
|425		| x13
|F-3/A		| x10
|SP 15D2	| x7
|10-QT/A	| x6
|S-3/A		| x5
|S-3ASR		| x4
|S-3		| x4
|F-3		| x3
|424B4		| x3
|DEF 14A	| x3
|20FR12B	| x3
|DEFA14A	| x2
|PRE 14A	| x2
|8-K12B/A	| x2
|486BPOS	| x2
|N-CSRS		| x2
|N-2/A		| x2
|PRER14C	| x2
|POSASR		| x1
|10-12B		| x1
|ANNLRPT	| x1
|F-3ASR		| x1
|18-K		| x1
|10-D		| x1
|NT 10-Q	| x1
|20FR12G	| x1
|N-CSR		| x1
|N-CSR/A	| x1
|N-2		| x1
|424B1		| x1
|PRER14A	| x1
|DEFM14C	| x1


## - Period
Balance Sheet Date, rounded to nearest month-end. (yyyymmdd)
- despite what the docs say, this can be null (in 6 cases)
  -> we just filter those out
- it is indeed always the last day of the month


## - Fy
- format: yyyy
- nulls: 9346


## - Fp
|||
|-|-
|FY		| x99831
|Q2		| x96981
|Q3		| x90995
|Q1		| x89692
|null	| x9618
|Q4		| x95
|H1		| x30
|CY		| x7
|T1		| x6
|M9		| x2
|H2		| x1


## - Prevrpt
|||
|-|-
|False	| x372226
|True	| x15032


## - Detail
|||
|-|-
|True	| x350978
|False	| x36280
