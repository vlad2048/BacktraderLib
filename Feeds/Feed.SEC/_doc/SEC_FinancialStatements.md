# Balance Sheet

```cs
Total assets                            					123
Total current assets
	Cash and short term investments
		Cash and cash equivalents
		Short term investments
	Receivables
	Inventory
	Other current assets
Total non-current assets
	Property, plant & equipment net
	Goodwill and intangible assets
		Goodwill
		Intangible assets
	Long term investments
	Other non-current assets
Other assets
Total liabilities and equity
Total liabilities
Current liabilities
	Payables
	Short term debt
	Deferred revenue
	Other current liabilities
Non-current liabilities
	Long term debt
	Other non-current liabilities
	Non-current deferred revenue
	Non-current deferred tax liabilities
Total equity
	Common stock
	Retained earnings
	Other stockholders equity
	Accumulated other comprehensive income loss
	Minority interest
```











# Trading 212
Values are for NVIDIA 2023: ```SEC-filings_NVIDIA.pdf```
<br><br><br>

```

       ╭────────────────── 2024 ───────────────────────╮
Nov Dec*Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec│Jan Feb
        --Q1'2014-- --Q2'2014-- --Q3'2014-- --Q4'2014--





       ╭────────────────── 2024 ───────────────────────╮
Nov Dec│Jan*Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec│Jan*Feb
            --Q1'2025-- --Q2'2025-- --Q3'2025-- --Q4'2025--

```







Parenthesised numbers means we filter these exceptions out.

| field | name | example | IsNull | IsNotLastDayOfMonth
|-|-|-|-|-
Sub.Period      | Balance Sheet Date (yyyymmdd)                 | 20240630                  | (6)   | 0
Sub.Fye         | Fiscal Year End Date (mmdd)                   | 0930                      | 199   | _
Sub.Fy          | Fiscal Year Focus                             | 2017                      | 9346  | _
Sub.Fp          | Fiscal Period Focus                           | FY, Q2, Q3, ...           | 9618  | _
Sub.Filed       | Filing date (yyyymmdd)                        | 20240807                  | 0     | _
Sub.Accepted    | Filing accepted (yyyy-mm-dd hh:mm:ss)         | 2024-08-07 17:20:00.0     | 0     | _
Num.Ddate       | End date for the data value                   | 20230630                  | 0     | (18)
Num.Qtrs        | Number of quarters for by the data value      | 2, 0                      | 0     | _

*Fy / Fiscal Year Focus*
- EDGAR XBRL Guide Ch. 3.1.8

*Fp / Fiscal Period Focus*
- EDGAR XBRL Guide Ch. 3.1.8 within Fiscal Year.

*Num.Ddate*
- The end date for the data value, rounded to the
nearest month end.

*Num.Qtrs*
- The count of the number of quarters
represented by the data value, rounded to the
nearest whole number. “0” indicates it is a
point-in-time value.




<br><br><br><br><br><br><br><br><br><br><br><br><br>

## Overview
Trading212 shows fundamental data across 3 panels and the field names can be a bit different:
- Change (histogram)
- Breakdown (flow chart)
- Details


## Income statement

| name (Trading212)     | kind      | value         | page (pdf)    | name (pdf)                    | table (pdf)                                       | XBRL Tag
|-|-|-|-|-|-|-
| Total revenue         | duration  | 60.922 B      | 51            | Revenue                       | Consolidated Statements of Income                 | ```Revenues```
| Gross profit          | duration  | 44.301 B      | 51            | Gross profit                  | Consolidated Statements of Income                 | ```GrossProfit```
| Cost of sales         | duration  | 16.621 B      | 51            | Cost of revenue               | Consolidated Statements of Income                 | ```CostOfRevenue```
| Operating profit      | duration  | 32.972 B      | 51            | Operating income              | Consolidated Statements of Income                 | ```OperatingIncomeLoss```
| Operating expenses    | duration  | 11.329 B      | 51            | Total operating expenses      | Consolidated Statements of Income                 | ```OperatingExpenses```

<br>

In Trading212, the change section can have different names:
- ```revenue``` means ```Total revenue```
- ```net income``` means ```Operating profit```
<br>


## Balance sheet

| name (Trading212)     | kind      | value         | page (pdf)    | name (pdf)                    | table (pdf)                                       | XBRL Tag
|-|-|-|-|-|-|-
| Current assets        | time      | 44.345 B      | 53            | Total current assets          | Consolidated Balance Sheets                       | ```AssetsCurrent```
| Non-current assets    |           | _
| Total assets          | time      | 65.728 B      | 53            | Total assets                  | Consolidated Balance Sheets                       | ```Assets``` or ```LiabilitiesAndStockholdersEquity```
| Total equity          | time      | 42.978 B      | 54            | Total Shareholders' Equity    | Consolidated Statements of Shareholders' Equity   | ```StockholdersEquity```
| Total liabilities     | time      | 22.750 B      | 53            | Total liabilities             | Consolidated Balance Sheets                       | ```Liabilities```

<br>

In Trading212, the change section can have different names:
- ```total assets``` means ```Total assets```
- ```total liabilities``` means ```Total liabilities```
<br>


## Cash flow

| name (Trading212)     | kind      | value         | page (pdf)    | name (pdf)                                            | table (pdf)                               | XBRL Tag
|-|-|-|-|-|-|-
| Operating activies    | duration  | 28.090 B      | 55            | Net cash provided by operating activities             | Consolidated Statements of Cash Flows     | ```NetCashProvidedByUsedInOperatingActivities```
| Investing activities  | duration  | -10.566 B     | 43            | Net cash provided by (used in) investing activities   | Liquidity and Capital Resources           | ```NetCashProvidedByUsedInInvestingActivities```
| Financing activities  | duration  | -13.633 B     | 43            | Net cash used in financing activities                 | Liquidity and Capital Resources           | ```NetCashProvidedByUsedInFinancingActivities```
|                       |           |               | 55            | Net cash provided by (used in) financing activities   | Consolidated Statements of Cash Flows     | 
| Change in cash        | duration  | 3.891 B       | 55            | Change in cash and cash equivalents                   | Consolidated Statements of Cash Flows     | (too long, see below)
<br>

The XBRL Tag for Change in cash is ```CashCashEquivalentsRestrictedCashAndRestrictedCashEquivalentsPeriodIncreaseDecreaseIncludingExchangeRateEffect```

<br>

In Trading212, the change section can have different names:
- ```operating cash flow``` means ```Operating activies```
- ```investing cash flow``` means ```Investing activities```
- ```financing cash flow``` means ```Financing activities```
<br>








<br><br><br><br>

```

1) download quarterly Financial Statement Data Sets
https://www.sec.gov/data-research/sec-markets-data/financial-statement-data-sets

2) Access specific CERT filings
https://www.sec.gov/edgar/eprints/request.html
(https://www.investor.gov/introduction-investing/investing-basics/glossary/public-documents)

3) EDGAR daily & full index AND MORE
https://www.sec.gov/Archives/edgar/daily-index/
https://www.sec.gov/Archives/edgar/full-index/
(https://www.sec.gov/search-filings/edgar-search-assistance/accessing-edgar-data)

4) EDGAR REST API & Bulk data
https://www.sec.gov/search-filings/edgar-application-programming-interfaces

5) XBRL
Description:           XBRL Index of EDGAR Dissemination Feed
Last Data Received:    January 7, 2025
Comments:              webmaster@sec.gov
Anonymous FTP:         ftp://ftp.sec.gov/edgar/

```