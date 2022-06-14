namespace InformBilling_JigsawClient_Example.Models
{
    public class SuccessReturnModelDto<T>
    {
        public T ReturnModel { get; set; }
        public long? JobNumber { get; set; }
        public bool MayRequireRerate { get; set; }

        public SuccessReturnModelDto(T returnModel, long? jobNumber = null, bool mayRequireRerate = false)
        {
            ReturnModel = returnModel;
            JobNumber = jobNumber;
            MayRequireRerate = mayRequireRerate;
        }

    }
}
