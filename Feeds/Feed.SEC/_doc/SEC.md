# Links
```
XBRL Rules
----------
https://xbrl.us/data-rule/dqc_0004/
```




<br><br><br><br><br>

# Process

```ps1

1 - Download
============
# folder: 1_download
# ram   : ?
# time  : ?
# files : 64 .zip
# size  : 4.74 gb

1) @if last scrape was too recent -> FINISH ('_last-check.json')
2) scrape links from 'https://www.sec.gov/data-research/sec-markets-data/financial-statement-data-sets'
3) download missing links
   ie:
     link: 'https://www.sec.gov/files/dera/data/financial-statement-data-sets/2009q1.zip'
     file: '1_download/2009q1.zip'

each zip contains 4 files:
    - sub.txt       2 mb    (report submissions)
    - num.txt     300 mb    (all numbers per report)
    - tag.txt      16 mb    (XBRL tags)
    - pre.txt      83 mb    (presentation information for tags & numbers)
    - readme.htm   13 kb


2 - Clean
=========
# folder: 2_clean
# ram   : 5 gb
# time  : 40 min
# files : 64 .zip
# size  : 4.54 gb

cleans each .zip in '1_download' into a .zip in '2_clean':

1) remove rows with invalid fields
2) remove rows with duplicate keys
3) remove rows with missing foreign reference keys
4) remove readme.htm
5) change row format
   before: tab and double quote escaping
   after : replace tab with '¬' (no more need for escaping)


3 - Group
=========
# folder: 3_group
# ram   : 3 gb
# time  : 134 min
# files : 21004 .zip
# size  : 2.46 gb

process each .zip in '2_clean' ('_quarters-done.json')

1) group the rows by company instead of by quarter
2) discards (blanks) the fields we dont need to fill our IRow records in memory
3) write a .zip per company in '3_group'


4 - Rename
==========
# folder: 4_rename
# ram   : ?
# time  : 8 min
# files : 16148 .zip
# size  : 2.42 gb

```




<br><br><br><br><br>

# Data

```
All characters used in all zip files (excluding the .htm file):

\t !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~



Keys
====
[sub]. ADSH
[tag].       TAG,VERSION
[num]. ADSH, TAG,VERSION, ddate,qtrs,uom,segments,coreg
[pre]. ADSH,              report,line


             ┌─ [num].ADSH
[sub].ADSH ←─┤
             └─ [pre].ADSH

                    ┌─ [num].TAG+VERSION
[tag].TAG+VERSION ←─┤
                    └─ [pre].TAG+VERSION


                       n   0
[num].ADSH+TAG+VERSION ←───→ [pre].ADSH+TAG+VERSION
```




<br><br><br>

# SEC Data Issues

```
Duplicate keys in [Sub]                                       Perfect!  (total:393749)
Duplicate keys in [Num]                                       Failures: 57/163038675  (0.0000%)
Duplicate keys in [Tag]                                       Perfect!  (total:4343445)
Duplicate keys in [Pre]                                       Perfect!  (total:41296419)
Missing reference from [Num] to [Sub]                         Failures: 1953/393749  (0.4960%)
Missing reference from [Pre] to [Sub]                         Perfect!  (total:393749)
Missing reference from [Num] to [Tag]                         Perfect!  (total:4343445)
Missing reference from [Pre] to [Tag]                         Failures: 16/4343445  (0.0004%)
Missing partial reference from [Num] to [Pre]                 Failures: 173/41296419  (0.0004%)
Missing partial reference from [Pre] to [Num]                 Failures: 6039/163038675  (0.0037%)
```

## Duplicate Num keys (Adsh, Tag, Version, DDate, Qtrs, Uom, Segments, Coreg)
```
Non unique keys for: NumRow
In C:\ProgramData\Feed.SEC\1_download\2011q1.zip/num.txt (lines: 2)
    [89863] 0000950123-11-020067        CommonStockDividendsPerShareDeclared    us-gaap/2009    20081231        0       USD                     0.4700  Rounded to the nearest penny. Exact dividend amount was $0.4666666 per common share and was reflective of a four-month period (December 2008 through March 2009), due to the change in the firm's fiscal year-end.
    [651978] 0000950123-11-020067       CommonStockDividendsPerShareDeclared    us-gaap/2009    20081231        0       USD                     0.4667

Non unique keys for: NumRow
In C:\ProgramData\Feed.SEC\1_download\2012q4.zip/num.txt (lines: 2)
    [249337] 0001193125-12-463716       NoninterestIncome       us-gaap/2012    20111231        1       USD                     8029000.0000
    [321371] 0001193125-12-463716       NoninterestIncome       us-gaap/2012    20111231        1       USD                     10489000.0000
```

## Missing reference from Num to Sub (Adsh)
```
1953 invalid reference from NumRow to SubRow
In C:\ProgramData\Feed.SEC\1_download\2013q1.zip/num.txt (lines: 1953)
    [985] 0001193125-13-087869  NetCashProvidedByUsedInInvestingActivities      us-gaap/2012    20111231        4       USD     LegalEntity=ParentCompany;      ParentCompany   -92855000.0000
    [1691] 0001193125-13-087866 StockIssuedDuringPeriodValueConversionOfConvertibleSecurities   us-gaap/2012    20121231        4       USD     EquityComponents=CommonStock;LegalEntity=Deerfield;       3000.0000
    [2170] 0001193125-13-087866 AmortizationOfIntangibleAssets  us-gaap/2012    20121231        4       USD     FiniteLivedIntangibleAssetsByMajorClass=ManufacturingFacilityProductionLicenses;          700000.0000
    (...)
```

## Missing reference from Pre to Tag (Tag, Version)
```
16 invalid reference from PreRow to TagRow
In C:\ProgramData\Feed.SEC\1_download\2013q1.zip/pre.txt (lines: 16)
    [131123] 0000897101-13-000282       4       39      IS      0       H       LoanAndCollectionExpense        0000897101-13-000282    Loan and collection expense     0
    [137961] 0000897101-13-000282       8       7       CF      0       H       AmortizationOfCoreDepositIntangible     0000897101-13-000282    Amortization of core deposit intangible 0
    [149369] 0000897101-13-000282       4       40      IS      0       H       OtherOutsideServices    0000897101-13-000282    Other outside services  0
    (...)
```




## Missing partial reference from Num to Pre (Adsh, Tag, Version)
```
477 invalid partial references from NumRow to PreRow (NumRowPartialKey)
In C:\ProgramData\Feed.SEC\1_download\2012q1.zip/num.txt (lines: 477)
    [5180] 0000025475-12-000032 PaymentsOfDividendsCommonStock  us-gaap/2011    20111231        4       USD     EquityComponents=NoncontrollingInterest;                0.0000
    [18282] 0000025475-12-000032        OtherComprehensiveIncomeDefinedBenefitPlansAdjustmentNetOfTaxPeriodIncreaseDecrease     us-gaap/2011    20111231        4       USD     EquityComponents=CommonClassA;             0.0000
    [21030] 0000025475-12-000032        StockIssuedDuringPeriodValueShareBasedCompensation      us-gaap/2011    20111231        4       USD     EquityComponents=CommonClassB;          0.0000
    (...)
```

## Missing partial reference from Pre to Num (Adsh, Tag, Version)
```
173 invalid partial references from PreRow to NumRow (NumRowPartialKey)
In C:\ProgramData\Feed.SEC\1_download\2013q1.zip/pre.txt (lines: 173)
    [8843] 0000897101-13-000282 8       61      CF      0       H       PaymentsOfDividendsCommonStock  us-gaap/2012    Cash dividends paid     1
    [18543] 0000897101-13-000282        8       12      CF      0       H       LifeInsuranceCorporateOrBankOwnedChangeInValue  us-gaap/2012    Net gain on life insurance death benefit        1
    [23621] 0000897101-13-000282        4       28      IS      0       H       IncomeLossFromEquityMethodInvestments   us-gaap/2012    Income in equity of UFS subsidiary      0
    (...)
```





<br><br><br>

## ```sub.txt``` (2 mb)
### Description
```
Submission data set; this includes one record for each XBRL submission with
amounts rendered by the Commission in the primary financial statements.
The set includes fields of information pertinent to the submission and the filing entity.
Information is extracted from the SEC’s EDGAR system and the filings submitted to the SEC by registrants.

SUB identifies all the EDGAR submissions with amounts rendered by the Commission on the
primary financial statements in the data set, with each row having the unique (primary) key adsh,
a 20 character EDGAR Accession Number with dashes in positions 11 and 14.
```


<br>

## ```num.txt``` (300 mb)
### Description
```
Number data set; this includes one row for each distinct amount appearing on the
primary financial statements rendered by the Commission from each submission included in the SUB data set.
```



<br>

## ```tag.txt``` (16 mb)
### Description
```
Tag data set; includes defining information about each numerical tag.
Information includes tag descriptions (documentation labels), taxonomy version information and other tag attributes.
```


<br>

## ```pre.txt``` (83 mb)
### Description
```
Presentation data set; this provides information about how the tags and numbers were presented
in the primary financial statements as rendered by the Commission.
```





<br><br><br><br><br>

# Step 1: Download quarterly zips
### Folder: ```1_download```
```
# ram  : ?
# time : ?
# files: 64 .zip
# size : 4.74 gb

1_download/_last-check.json         (2025-03-24 02:54:09)
          /2020q1.zip      400 mb
          /2020q2.zip
           ...
```

Download the quarterly zips into this folder.
Save the last time we checked for new links into ```_last-check.json``` to avoid scraping that page too often

```
source              https://www.sec.gov/data-research/sec-markets-data/financial-statement-data-sets
example link        https://www.sec.gov/files/dera/data/financial-statement-data-sets/2009q1.zip

each zip contains 4 files:
    - num.txt     300 mb
    - pre.txt     83 mb
    - sub.txt     2 mb
    - tag.txt     16 mb
```



<br><br><br>

# Step 2: Clean zips into new zips
### Folder: ```2_clean```
```
# ram  : [3.7 - 5.3] gb
# time : 50 min
# files: 64 .zip
# size : 4.54 gb

2_clean/2020q1.zip      90 mb
       /2020q2.zip
```




<br><br><br>

# Step 3: Group by company
### Folder: ```3_read```
```
# ram  : ?
# time : 90 min
# files: 21000 .zip
# size : ?

3_read/_quarters-done.json                  (2020q1 \n 2020q2 \n ...)
      /1 800 FLOWERS COM INC.zip
      /4CABLE TV INTERNATIONAL, INC~
                             ...
```

Read the datapoints from the zip files
And save them in ```[company]/[quarter].json``` files

Keep track of which quarters from 1_download we've processed in the
```_quarters-done.json``` file



<br><br><br>

# Step 3: Process
### Folder: ```3_process```
```
# 20000 .json files
3_process/1 800 FLOWERS COM INC.json      ? mb
         /1LIFE HEALTHCARE INC.json
```

Write 1 .json file per company that contains all its
- balance sheets
- income statements
- cash flows



<br><br><br>

# Pdfs folder
### Folder: ```pdfs```
```
pdfs/BONE BIOLOGICS, CORP/0001493152-14-003110.pdf      5570 kb
                         /0001493152-15-003724.pdf
```
Used to store downloaded .pdf files with a subfolder for each company
