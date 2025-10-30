namespace SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;

internal enum ApprenticeshipStatus
{
    Live,
    WaitingToStart,
    Paused,
    Stopped,
    Completed
}

internal enum FundingType
{
    DirectTransferFundsFromConnection,
    ReservedFunds,
    ReserveNewFunds,
    CurrentLevyFunds,
    TransferFunds
}

public enum EmployerType
{
    Levy,
    NonLevy,
    NonLevyUserAtMaxReservationLimit
}

public enum ApprenticeRequests
{
    ReadyForReview,
    WithEmployers,
    WithTrainingProviders,
    Drafts,
    WithTransferSendingEmployers
}

public enum ProviderUserRoles
{
    Contributor,
    ContributorWithApproval,
    AccountOwner,
    Viewer
}
