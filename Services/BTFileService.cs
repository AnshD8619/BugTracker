using BugTracker.Data;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTFileService : IBTFileService
    {
        #region Variables
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        #endregion

        #region ConvertByteArrayToFile
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData); // Gets imageBase64Data by converting file data to a string of base 64
                return string.Format($"data:image/{extension};base64,{imageBase64Data}"); // Returns string formatted with extension and imageBase64Data
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region ConvertFileToByteArrayAsync
        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(); // Initializes memoryStream of type MemoryStream
                await file.CopyToAsync(memoryStream); // Uploads file content to memory stream to be backed up and stored
                var byteFile = memoryStream.ToArray(); // Turns the stream to a byte array
                memoryStream.Close(); // Closes memory stream
                memoryStream.Dispose(); // Releases resources in memory stream
                return byteFile;
            }

            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region FormatFileSize
        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024; // Divides fileSize by bytes through every loop
                counter++; // Adds 1 to counter through every loop
            }

            return String.Format("{0:n1}{1}", number, suffixes[counter]); // Returns a string formatted with the fileSize and suffixes[counter]
        }
        #endregion

        #region GetFileIcon
        public string GetFileIcon(string file)
        {
            string ext = Path.GetExtension(file).Replace(".", "");
            return $"/img/png/{ext}.png";
        } 
        #endregion
    }
}
