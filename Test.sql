SELECT TOP (1000)
       *
FROM [Hypernova].[dbo].[Creditors]
ORDER BY 1 DESC;

SELECT TOP (1000)
       *
FROM [Hypernova].[dbo].[Claims]
ORDER BY 1 DESC;

SELECT TOP (1000)
       *
FROM [Hypernova].[dbo].[ClaimItems]
ORDER BY 1 DESC;

SELECT TOP (1000)
       *
FROM [Hypernova].[dbo].[Invoices]
ORDER BY 1 DESC;

SELECT *
FROM [dbo].[Claims]
    INNER JOIN [dbo].[ClaimItems]
        ON [ClaimId] = [Claims].[Id]
           AND [Type] = 1
    INNER JOIN [dbo].[Invoices]
        ON [ClaimItemId] = [ClaimItems].[Id];

SELECT *
FROM [dbo].[Claims]
    INNER JOIN [dbo].[ClaimItems]
        ON [ClaimId] = [Claims].[Id]
           AND [Type] = 2
    INNER JOIN [dbo].[CreditNotes]
        ON [ClaimItemId] = [ClaimItems].[Id];

SELECT *
FROM [Debtors]
    INNER JOIN [DebtorNaturalPersons]
        ON [DebtorNaturalPersons].[DebtorId] = [Debtors].[Id];

SELECT *
FROM [Debtors]
    INNER JOIN [DebtorNaturalPersons]
        ON [DebtorNaturalPersons].[DebtorId] = [Debtors].[Id]
           AND [Debtors].[Type] = 1
    INNER JOIN [dbo].[ClaimDebtors]
        ON [ClaimDebtors].[DebtorId] = [Debtors].[Id]
    INNER JOIN [dbo].[Claims]
        ON [Claims].[Id] = [ClaimDebtors].[ClaimId];

