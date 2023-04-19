-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rafael Aguero
-- Create date: 04/18/2023
-- Description:	set users as bad payer
-- =============================================
CREATE PROCEDURE SetBadPayers
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	UPDATE AspNetUsers
	SET BadPayer = 1
	from Invoices as i
	join AspNetUsers u on  u.id = i.UserId
	where i.Payed = 0 and i.PaydayLimitDate < GETDATE()
END
GO
