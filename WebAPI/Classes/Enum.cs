
namespace WebAPI.Classes
{
    public static class Enum
    {
        public enum NotificationMethod { Email = 1, Push = 2 }
        public enum TaskType {
            ReviewOffer = 1,
            PayPreviewFee = 2, 
            PostPreview = 3,
            MakePaymentHold = 4,
            PostToPlatform = 5,
            ReviewPreview = 6
        }
        public enum TaskStatus { Assigned = 1, Completed = 2, Deleted = 3 }
        public enum ContractState {
            Offer = 1,
            CounterOffer = 2,
            PreviewPayment = 3,
            PostPreview = 4,
            PaymentHold = 5,
            PostToPlatform = 6,
            Payment = 7,
            ContractClosedCancelled = 8,
            PreviewReview = 10,
            ContractClosedOfferRejected = 11
        }

        public enum ContractStateResult
        {
            //OfferAccepted = 1,
            //OfferRejected = 2,
            //PreviewPosted = 13
            AcceptOffer = 1,
            RejectOffer = 2,
            PostPreview = 13,
            AcceptPreview = 23,
            RejectPreview = 24

        }

        public enum Role { Influencer, Advertiser }

        public enum NotificationTemplates
        {
            ReviewOffer = 4,
            PostPreview = 5,
            PaymentHold = 6,
            OfferAccepted = 7,
            OfferRejected = 8,
            ReviewPreview = 9,
            PostToPlatform = 10,
            RepostPreview = 11
        }

        public enum PreviewResults
        {
            Accepted = 1,
            Rejected =2
        }

        public enum CampaignInfluencerStatuses
        {
            Invited = 1,
            AcceptedInvite = 2,
            RejectedInvite = 3
        }

        public enum CampaignInfluencerNonRegStatuses
        {
            New = 1,
            InviteInfluencer = 2,
            AgreementInProgress = 3,
            PostInProgress = 4,
            PaymentInProgress = 5,
            Completed = 6,
            Closed = 7
        }





        //public enum NotificationTemplate
        //{
        //    InfluencerInviteRequested = 1,
        //    InfluencerInviteAccepted = 2,
        //    InfluencerActivated = 3,
        //    ReviewOffer = 4
        //}
    }
}
