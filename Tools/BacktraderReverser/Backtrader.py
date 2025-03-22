import datetime as dt
import numpy as np
import pandas as pd
import backtrader as bt
import math


class LoggerMain:
    def __init__(self):
        self.buf = f'# Main\nDate,Open,High,Low,Close,Equity,Cash'
    def log(self, strat: bt.Strategy):
        broker: bt.BackBroker = strat.broker
        str = f'''
{strat.data.datetime.date()},\
{strat.data.open[0]},\
{strat.data.high[0]},\
{strat.data.low[0]},\
{strat.data.close[0]},\
{broker.get_value()},\
{broker.cash}'''
        #print(str)
        self.buf += str



class LoggerOrder:
    def __init__(self):
        self.buf = f'\n# Orders\nDate,Status,Dir,Size,Type,Price,PriceLimit,ExecSize,ExecPrice,ExecComm'
    def log(self, strat: bt.Strategy, order: bt.Order):
        orderPriceStr = '_' if order.price is None else f'{order.price}'
        orderPriceLimitStr = '_' if order.plimit is None else f'{order.plimit}'
        str = f'''
{strat.data.datetime.date()},\
{order.getstatusname()},\
{order.ordtypename()},\
{order.size},\
{bt.Order.ExecTypes[order.exectype]},\
{orderPriceStr},\
{orderPriceLimitStr},\
{order.executed.size},\
{order.executed.price},\
{order.executed.comm}'''
        #print(str)
        self.buf += str



class LoggerTrade:
    def __init__(self):
        self.buf = f'\n# Trades\nDate,Status,IsLong,Size,Price,PnL,Comm'
    def log(self, strat: bt.Strategy, trade: bt.Trade):
        str = f'''
{strat.data.datetime.date()},\
{bt.Trade.status_names[trade.status]},\
{trade.long},\
{trade.size},\
{trade.price},\
{trade.pnl},\
{trade.pnlcomm}'''
        #print(str)
        self.buf += str



logMain = LoggerMain()
logOrder = LoggerOrder()
logTrade = LoggerTrade()



class SingleOrder(bt.Strategy):
    def next(self):
        logMain.log(self)
        #if self.data.datetime.date() == dt.date(2024, 1, 1):
        #    self.buy(size=10, exectype=bt.Order.Market)
        #if self.data.datetime.date() == dt.date(2024, 1, 3):
        #    self.sell(size=10, exectype=bt.Order.Market)
__orders__
    def notify(self, order: bt.Order):
        logOrder.log(self, order)
    def notify_trade(self, trade: bt.Trade):
        logTrade.log(self, trade)

cerebro = bt.Cerebro(stdstats = True)
cerebro.adddata(bt.feeds.PandasData(dataname=pd.DataFrame(
    {
        #'Open'  : [3, 5, 18, 7, 6, 5, 4],
        #'Close' : [3, 5, 8, 7, 6, 5, 4],
        #'High'  : [3, 5, 8, 7, 6, 5, 4],
        #'Low'   : [3, 5, 8, 7, 6, 5, 4],
__prices__
    },
    index=pd.date_range('__startDate__', periods=__pricesLength__, freq='d', name='Date')
)))
cerebro.addstrategy(SingleOrder)
broker: bt.BackBroker = cerebro.broker
broker.setcash(__brokerCash__)
broker.setcommission(commission=__brokerComm__, leverage=1)
broker.set_coc(__cheatOnClose__)
strats = cerebro.run()

print(logMain.buf)
print(logOrder.buf)
print(logTrade.buf)
