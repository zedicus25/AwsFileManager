using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Aws.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Aws
{
    public class AwsController
    {
        private AmazonS3Client _s3Client;
        private AmazonRekognitionClient _amazonRekognition;
        private readonly string _bucketName;
        private readonly RegionEndpoint _region;

        public AwsController()
        {
            string accesKey = ConfigurationManager.ConnectionStrings["accessKey"].ConnectionString;
            string secretKey = ConfigurationManager.ConnectionStrings["secretKey"].ConnectionString;
            //_bucketName = ConfigurationManager.ConnectionStrings["bucketName"].ConnectionString;
            _bucketName = ConfigurationManager.ConnectionStrings["rekigBucketName"].ConnectionString;
            _region = RegionEndpoint.EUWest2;
            _s3Client = new AmazonS3Client(accesKey, secretKey, _region);
            _amazonRekognition = new AmazonRekognitionClient(accesKey, secretKey, _region);
        }

        public List<AwsFile> ListBucketContents()
        {
            List<AwsFile> files = new List<AwsFile>();
            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    MaxKeys = 35,
                };
                var response = new ListObjectsV2Response(); 
                do
                {
                    response = _s3Client.ListObjectsV2(request); 
                    response.S3Objects
                    .ForEach(obj => files.Add(new AwsFile(obj.LastModified, obj.Size, obj.Key)));                                                                                                                                    // from the NextContinuationToken property of the response.
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);
            }
            catch (Exception)
            {
            }

            return files;
        }

        public bool DownloadFile(string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
            };     // Issue request and remember to dispose of the response
            GetObjectResponse response =  _s3Client.GetObject(request);
            try
            {
                // Save object to local file
                response.WriteResponseStreamToFile($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{fileName}", true);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                }; 
                _s3Client.DeleteObject(deleteObjectRequest);
                return true;
      
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UploadFile(string fileName, string filePath)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                FilePath = filePath,
            }; 
            var response = _s3Client.PutObject(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public string GetLinesFromImage(string pic)
        {
            string fileInfo = "";

            DetectTextRequest detectTextRequest = new DetectTextRequest()
            {
                Image = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Name = pic,
                        Bucket = _bucketName
                    }
                }
            };

            try
            {
                DetectTextResponse detectTextResponse = _amazonRekognition.DetectTextAsync(detectTextRequest).GetAwaiter().GetResult();
                fileInfo += "Detected lines for " + pic + "\n";
                foreach (TextDetection text in detectTextResponse.TextDetections.Where(x => x.Type == TextTypes.LINE))
                {
                    fileInfo += text.DetectedText + " ";
                }
            }
            catch (Exception e)
            {
            }
            return fileInfo;
        }
        public string GetWordsFromImage(string pic)
        {
            string fileInfo = "";

            DetectTextRequest detectTextRequest = new DetectTextRequest()
            {
                Image = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Name = pic,
                        Bucket = _bucketName
                    }
                }
            };

            try
            {
                DetectTextResponse detectTextResponse = _amazonRekognition.DetectTextAsync(detectTextRequest).GetAwaiter().GetResult();
                fileInfo += "Detected words for " + pic + "\n";
                foreach (TextDetection text in detectTextResponse.TextDetections.Where(x => x.Type == TextTypes.WORD))
                {
                    fileInfo += text.DetectedText + " ";
                }
            }
            catch (Exception e)
            {
            }
            return fileInfo;
        }
    }
}
