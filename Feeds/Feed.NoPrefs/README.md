# Basics
```
Number of rows
==============
symbol		20543
split		2772
dividend	437206
ohlcv		24252896

Splits
======
Forward_GTE_2	2119
Backward		631
Forward_LT_2	15
Invalid			7

Nullability (nothing means not null)
====================================
split.to_factor
split.for_factor

dividend.amount

ohlcv.open
ohlcv.high
ohlcv.low
ohlcv.close
ohlcv.volume

symbol.security_name
symbol.listing_exchange
symbol.market_category			NULL
symbol.is_etf					NULL
symbol.round_lot_size
symbol.is_test_issue
symbol.financial_status			NULL
symbol.cqs_symbol				NULL
symbol.nasdaq_symbol
symbol.is_next_shares
symbol.last_seen
```

# symbols
```
ListingExchange
===============
	NASDAQ		9668
	NYSE		5762
	NYSE ARCA	3252
	BATS		1201
	NYSE MKT	657
	IEXG		3

MarketCategory
==============
	null			10875
	Capital			4457
	Global			2751
	Global Select	2460

IsEtf
=====
	0		15258
	1		5275
	null	10

RoundLotSize
============
	100		20522
	10		11
	1		10

IsTestIssue
===========
	0		20509
	1		34

FinancialStatus
===============
	null									10875
	Normal									8029
	Deficient								1368
	Deficient and Delinquent				146
	Delinquent								101
	Deficient and Bankrupt					11
	Bankrupt								8
	Deficient, Delinquent, and Bankrupt		4
	Delinquent and Bankrupt					1

IsNextShares
============
	0		20524
	1		19
```





# Dolt usage

```https://docs.dolthub.com/cli-reference/cli```


## Configure
```ps1
dolt config --global --add user.name "Vlad Niculescu"
dolt config --global --add user.email "vlad.nic2@gmail.com"
dolt config --list
```


## Clone database
```ps1
dolt clone https://www.dolthub.com/repositories/post-no-preference/rates
cd rates
```


## Updating database
```ps1
dolt fetch
dolt pull
dolt status
```


## Using database
```ps1
dolt sql-server

# setup MySql db in LINQPad using
# Connection string: server=localhost;user id=root;database=stocks
```


## Misc
```ps1
# show schemas
dolt schema show

# export table as .csv
dolt table export symbol symbol.csv

# export table as .json
dolt table export symbol symbol.json
```
