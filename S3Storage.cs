using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace dackup
{
    public class S3Storage : StorageBase
    {
        private string region, bucket, accessKeyId, accessKeySecret, pathPrefix;

        private S3Storage(){}

        
        public DateTime? RemoveThreshold {get;set;}
        public string PathPrefix{get;set;}
        
        public S3Storage(string region, string accessKeyId, string accessKeySecret, string bucket)
        {
            this.region = region;
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
            this.bucket = bucket;
        }
        protected override UploadResult Upload(string fileName)
        {    
            return new UploadResult();
        }

        protected override PurgeResult Purge()
        {
            if (RemoveThreshold == null || RemoveThreshold.Value > DateTime.Now)
            {
                return new PurgeResult();
            }
            return new PurgeResult();
        }
    }
}