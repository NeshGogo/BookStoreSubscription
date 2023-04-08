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
-- Create date: 04/08/2023
-- Description:	genererate the invoices
-- =============================================
CREATE PROCEDURE InvoiceCreation
	-- Add the parameters for the stored procedure here
	@beginDate datetime,
	@endDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @amountByPetition decimal(4,4) = 1.0/2 -- 2.0/1000 -- 2 dollars by every 1000 petition;

	INSERT INTO Invoices(UserId, Amount, IssueDate, Payed, PaydayLimitDate)
	SELECT 
		k.UserId,
		COUNT(*) * @amountByPetition as Amount,
		GETDATE() IssueDate,
		0 as Payed,
		DATEADD(d, 60, GETDATE()) AS PaydayLimitDate
	FROM Petitions P
	JOIN KeyAPIs K ON K.Id = P.KeyAPIId
	WHERE K.KeyType != 1 AND P.PetitionDate >= @beginDate AND P.PetitionDate < @endDate
	GROUP BY K.UserId

	INSERT INTO InvoiceIssueds(Month, Year)
	select
		case MONTH(GETDATE())
		when 1 then 12
		else
		MONTH(GETDATE()) -1 end as month , 
	
		case MONTH(GETDATE())
		when 1 then YEAR(GETDATE())-1
		else
		YEAR(GETDATE()) end as year
END
GO
