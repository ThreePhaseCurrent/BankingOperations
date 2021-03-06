USE [bank_operations]
GO
/****** Object:  UserDefinedFunction [dbo].[GetTypeClient]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetTypeClient](@id INT)
RETURNS NVARCHAR(MAX)
BEGIN

declare @s VARCHAR(100);
declare @txt NVARCHAR(MAX);
set @txt = '';

IF(EXISTS(SELECT * FROM physical_person WHERE id_person = @id))
	set @txt = 'Физическое лицо'
ELSE set @txt = 'Юридическое лицо'

RETURN @txt

END
GO
/****** Object:  Table [dbo].[currency]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[currency](
	[id_currency] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](30) NOT NULL,
	[short_name] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_currency] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[bank_account]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bank_account](
	[id_account] [int] IDENTITY(1,1) NOT NULL,
	[id_client] [int] NOT NULL,
	[account_type] [varchar](20) NULL,
	[id_currency] [int] NOT NULL,
	[date_open] [date] NOT NULL,
	[date_close] [date] NULL,
	[amount] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK__bank_acc__B2C7C783FDC62652] PRIMARY KEY CLUSTERED 
(
	[id_account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[FindAccountByCurrencyType]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FindAccountByCurrencyType](@typecurrency VARCHAR(30))
RETURNS TABLE 
AS 
RETURN (SELECT id_account AS Код_аккаунта, id_client AS Код_клиента, account_type AS Тип_аккаунта, 
currency.name AS Валюта, date_open AS Дата_открытия, date_close AS Дата_закрытия, 
amount AS Сумма FROM bank_account INNER JOIN currency ON bank_account.id_currency = currency.id_currency WHERE bank_account.id_currency IN (SELECT id_currency FROM currency WHERE name = @typecurrency))
GO
/****** Object:  Table [dbo].[operation]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[operation](
	[operation_time] [datetime] NOT NULL,
	[id_account] [int] NOT NULL,
	[type_operation] [varchar](30) NOT NULL,
	[amount] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_operation] PRIMARY KEY CLUSTERED 
(
	[operation_time] ASC,
	[id_account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[FindAccountOperationByType]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FindAccountOperationByType](@idaccount INT, @operationtype VARCHAR(30))
RETURNS TABLE 
AS 
RETURN (SELECT operation.id_account AS Код_аккаунта, operation.operation_time AS Время_операции, operation.type_operation AS Тип_операции, operation.amount AS Сума FROM operation WHERE id_account = @idaccount AND type_operation = @operationtype)
GO
/****** Object:  UserDefinedFunction [dbo].[FindOpenAccountRange]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FindOpenAccountRange](@startdate DATETIME, @finishdate DATETIME)
RETURNS TABLE 
AS 
RETURN (SELECT id_account AS Код_аккаунта, id_client AS Код_клиента, account_type AS Тип_аккаунта, 
currency.name AS Валюта, date_open AS Дата_открытия, date_close AS Дата_закрытия, 
amount AS Сумма FROM bank_account INNER JOIN currency ON bank_account.id_currency = currency.id_currency WHERE date_open <= @finishdate AND date_open >= @startdate)
GO
/****** Object:  UserDefinedFunction [dbo].[FindCloseAccountRange]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[FindCloseAccountRange](@startdate DATETIME, @finishdate DATETIME)
RETURNS TABLE 
AS 
RETURN (SELECT id_account AS Код_аккаунта, id_client AS Код_клиента, account_type AS Тип_аккаунта, 
currency.name AS Валюта, date_open AS Дата_открытия, date_close AS Дата_закрытия, 
amount AS Сумма FROM bank_account INNER JOIN currency ON bank_account.id_currency = currency.id_currency WHERE date_close <= @finishdate AND date_close >= @startdate)
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Accrual]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accrual](
	[id_accrual] [int] IDENTITY(1,1) NOT NULL,
	[accrual_date] [date] NOT NULL,
	[accrual_amount] [decimal](10, 2) NOT NULL,
	[id_deposit] [int] NOT NULL,
 CONSTRAINT [PK_Accrual] PRIMARY KEY CLUSTERED 
(
	[id_accrual] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[client]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[client](
	[id_client] [int] IDENTITY(1,1) NOT NULL,
	[login] [varchar](100) NOT NULL,
	[password] [varchar](24) NULL,
	[address] [varchar](120) NOT NULL,
	[tel_number] [varchar](15) NOT NULL,
 CONSTRAINT [PK__client__6EC2B6C0AA0D35CD] PRIMARY KEY CLUSTERED 
(
	[id_client] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[credit]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[credit](
	[id_credit] [int] IDENTITY(1,1) NOT NULL,
	[id_account] [int] NOT NULL,
	[percent_credit] [decimal](10, 2) NOT NULL,
	[amount] [decimal](10, 2) NOT NULL,
	[date_credit] [date] NOT NULL,
	[date_credit_finish] [date] NOT NULL,
	[status] [bit] NOT NULL,
 CONSTRAINT [PK_credit] PRIMARY KEY CLUSTERED 
(
	[id_credit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[deposit]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[deposit](
	[id_deposit] [int] IDENTITY(1,1) NOT NULL,
	[id_account] [int] NOT NULL,
	[percent_deposit] [decimal](10, 2) NOT NULL,
	[amount] [decimal](10, 2) NOT NULL,
	[date_deposit] [date] NOT NULL,
	[date_deposit_finish] [date] NOT NULL,
	[status] [bit] NOT NULL,
 CONSTRAINT [PK_deposit] PRIMARY KEY CLUSTERED 
(
	[id_deposit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[exchange_rate]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[exchange_rate](
	[date_rate] [date] NOT NULL,
	[id_currency] [int] NOT NULL,
	[rate_sale] [decimal](10, 5) NOT NULL,
	[rate_buy] [decimal](10, 5) NOT NULL,
 CONSTRAINT [PK__exchange__180651E1A5EECBD7] PRIMARY KEY CLUSTERED 
(
	[date_rate] ASC,
	[id_currency] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[legal_person]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[legal_person](
	[id_edrpou] [int] NOT NULL,
	[name] [varchar](100) NOT NULL,
	[ownership_type] [varchar](20) NOT NULL,
	[director] [varchar](50) NOT NULL,
 CONSTRAINT [PK_legal_person] PRIMARY KEY CLUSTERED 
(
	[id_edrpou] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payment]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payment](
	[id_payment] [int] IDENTITY(1,1) NOT NULL,
	[date_payment] [date] NOT NULL,
	[amount_payment] [decimal](10, 2) NOT NULL,
	[id_credit] [int] NOT NULL,
 CONSTRAINT [PK_payment] PRIMARY KEY CLUSTERED 
(
	[id_payment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[physical_person]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[physical_person](
	[id_person] [int] NOT NULL,
	[passport_series] [varchar](2) NOT NULL,
	[passport_number] [varchar](10) NOT NULL,
	[identification_number] [varchar](10) NOT NULL,
	[surname] [varchar](50) NOT NULL,
	[name] [varchar](40) NOT NULL,
	[patronymic] [varchar](80) NOT NULL,
 CONSTRAINT [PK_physical_person] PRIMARY KEY CLUSTERED 
(
	[id_person] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accrual]  WITH CHECK ADD  CONSTRAINT [FK_Accrual_deposit] FOREIGN KEY([id_deposit])
REFERENCES [dbo].[deposit] ([id_deposit])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Accrual] CHECK CONSTRAINT [FK_Accrual_deposit]
GO
ALTER TABLE [dbo].[bank_account]  WITH CHECK ADD  CONSTRAINT [FK__bank_acco__id_cl__412EB0B6] FOREIGN KEY([id_client])
REFERENCES [dbo].[client] ([id_client])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[bank_account] CHECK CONSTRAINT [FK__bank_acco__id_cl__412EB0B6]
GO
ALTER TABLE [dbo].[bank_account]  WITH CHECK ADD  CONSTRAINT [FK__bank_acco__id_cu__4316F928] FOREIGN KEY([id_currency])
REFERENCES [dbo].[currency] ([id_currency])
GO
ALTER TABLE [dbo].[bank_account] CHECK CONSTRAINT [FK__bank_acco__id_cu__4316F928]
GO
ALTER TABLE [dbo].[credit]  WITH CHECK ADD  CONSTRAINT [FK_credit_bank_account] FOREIGN KEY([id_account])
REFERENCES [dbo].[bank_account] ([id_account])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[credit] CHECK CONSTRAINT [FK_credit_bank_account]
GO
ALTER TABLE [dbo].[deposit]  WITH CHECK ADD  CONSTRAINT [FK_deposit_deposit] FOREIGN KEY([id_account])
REFERENCES [dbo].[bank_account] ([id_account])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[deposit] CHECK CONSTRAINT [FK_deposit_deposit]
GO
ALTER TABLE [dbo].[exchange_rate]  WITH CHECK ADD  CONSTRAINT [FK__exchange___id_cu__45F365D3] FOREIGN KEY([id_currency])
REFERENCES [dbo].[currency] ([id_currency])
GO
ALTER TABLE [dbo].[exchange_rate] CHECK CONSTRAINT [FK__exchange___id_cu__45F365D3]
GO
ALTER TABLE [dbo].[legal_person]  WITH CHECK ADD  CONSTRAINT [FK_LegalPerson_Client] FOREIGN KEY([id_edrpou])
REFERENCES [dbo].[client] ([id_client])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[legal_person] CHECK CONSTRAINT [FK_LegalPerson_Client]
GO
ALTER TABLE [dbo].[operation]  WITH CHECK ADD  CONSTRAINT [FK__operation__id_ac__47DBAE45] FOREIGN KEY([id_account])
REFERENCES [dbo].[bank_account] ([id_account])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[operation] CHECK CONSTRAINT [FK__operation__id_ac__47DBAE45]
GO
ALTER TABLE [dbo].[payment]  WITH CHECK ADD  CONSTRAINT [FK_payment_credit] FOREIGN KEY([id_credit])
REFERENCES [dbo].[credit] ([id_credit])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[payment] CHECK CONSTRAINT [FK_payment_credit]
GO
ALTER TABLE [dbo].[physical_person]  WITH CHECK ADD  CONSTRAINT [FK_physical_person_client] FOREIGN KEY([id_person])
REFERENCES [dbo].[client] ([id_client])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[physical_person] CHECK CONSTRAINT [FK_physical_person_client]
GO
ALTER TABLE [dbo].[bank_account]  WITH CHECK ADD  CONSTRAINT [CK__bank_acco__accou__4222D4EF] CHECK  (([account_type]='специальный' OR [account_type]='карточный' OR [account_type]='лицевой' OR [account_type]='депозитный' OR [account_type]='текущий' OR [account_type]='ссудный' OR [account_type]='расчетный' OR [account_type]='кредитный'))
GO
ALTER TABLE [dbo].[bank_account] CHECK CONSTRAINT [CK__bank_acco__accou__4222D4EF]
GO
/****** Object:  StoredProcedure [dbo].[AccountingForeignСurrency]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountingForeignСurrency](@id_account INT)
AS
BEGIN
	SELECT operation.operation_time AS Время_операции, operation.id_account AS Код_аккаунта, 
	name AS Валюта, type_operation AS Тип_операции, operation.amount AS Сумма 
		FROM bank_account 
		INNER JOIN operation ON operation.id_account = bank_account.id_account 
		INNER JOIN currency ON bank_account.id_currency = currency.id_currency 
		WHERE bank_account.id_currency = 1 OR bank_account.id_currency = 3 AND bank_account.id_account = @id_account
END
GO
/****** Object:  StoredProcedure [dbo].[AccountingForeignСurrencyByPreiod]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AccountingForeignСurrencyByPreiod](@id_account INT, @startDate DATE, @finishDate DATE)
AS
BEGIN
	SELECT operation.operation_time AS Время_операции, operation.id_account AS Код_аккаунта, 
	name AS Валюта, type_operation AS Тип_операции, operation.amount AS Сумма 
		FROM bank_account 
		INNER JOIN operation ON operation.id_account = bank_account.id_account 
		INNER JOIN currency ON bank_account.id_currency = currency.id_currency 
		WHERE bank_account.id_currency = 1 OR bank_account.id_currency = 3 
			AND bank_account.id_account = @id_account AND operation.operation_time >= @startDate 
			AND operation.operation_time <= @finishDate
END
GO
/****** Object:  StoredProcedure [dbo].[AddClient]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddClient](@login VARCHAR(100), @password VARCHAR(24), @address VARCHAR(120), @tel VARCHAR(15))
AS
	BEGIN
		INSERT INTO client VALUES(@login, @password, @address, @tel)
	END
GO
/****** Object:  StoredProcedure [dbo].[AddExchangeRate]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddExchangeRate](@name VARCHAR(30), @sale DECIMAL(10,5), @buy DECIMAL(10,5))
AS
BEGIN
	DECLARE @id INT = (SELECT id_currency FROM currency WHERE name = @name)

	IF(EXISTS(SELECT id_currency FROM currency WHERE name = @name))
		INSERT INTO exchange_rate VALUES (CAST(CAST(GETDATE() AS date) as DATETIME), @id, @sale, @buy)

END
GO
/****** Object:  StoredProcedure [dbo].[CloseAccount]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CloseAccount](@idClient INT)
AS
BEGIN
	IF((SELECT F = CASE WHEN date_close IS NULL THEN 1 ELSE 2 END FROM bank_account WHERE id_account = @idClient) = 1)
		UPDATE bank_account SET date_close = GETDATE() WHERE id_account = @idClient
END
GO
/****** Object:  StoredProcedure [dbo].[CreateAccount]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateAccount](@id_client INT, @account_type VARCHAR(20), 
	@currency VARCHAR(30), @amount DECIMAL(10,2))
AS
BEGIN
	DECLARE @id_currency INT;

	DECLARE CUR CURSOR FOR SELECT id_currency FROM currency WHERE name = @currency

	OPEN CUR;
		FETCH NEXT FROM CUR INTO @id_currency;

		INSERT INTO bank_account VALUES (@id_client, @account_type, @id_currency, GETDATE(), NULL, @amount)
	CLOSE CUR;
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteAccount]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteAccount](@id_account INT)
AS
BEGIN
	DELETE FROM bank_account WHERE id_account = @id_account
END
GO
/****** Object:  StoredProcedure [dbo].[FindTypeAccount]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[FindTypeAccount](@type VARCHAR(20))
AS
BEGIN
	SELECT id_account AS Код_аккаунта, id_client AS Код_клиента, account_type AS Тип_аккаунта, 
	currency.name AS Валюта, date_open AS Дата_открытия, date_close AS Дата_закрытия, 
	amount AS Сумма FROM bank_account INNER JOIN currency ON bank_account.id_currency = currency.id_currency WHERE account_type = @type
END
GO
/****** Object:  StoredProcedure [dbo].[MoneyTransfer]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MoneyTransfer](@id INT, @sum DECIMAL(10,5))
AS
BEGIN
	DECLARE @lastSum DECIMAL(10,5) = (SELECT amount FROM bank_account WHERE id_account = @id)

	UPDATE bank_account SET amount = @lastSum + @sum WHERE id_account = @id
	INSERT INTO operation VALUES(GETDATE(), @id, 'зачисление', @sum)
END
GO
/****** Object:  StoredProcedure [dbo].[WithdrawalFunds]    Script Date: 18.05.2020 23:31:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[WithdrawalFunds](@id INT, @sum DECIMAL(10,5))
AS
BEGIN
	DECLARE @lastSum DECIMAL(10,5) = (SELECT amount FROM bank_account WHERE id_account = @id)

	UPDATE bank_account SET amount = @lastSum - @sum WHERE id_account = @id
	INSERT INTO operation VALUES(GETDATE(), @id, 'снятие', @sum)
END
GO
