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
                return string.Format($"data: {extension};base64,{imageBase64Data}"); // Returns string formatted with extension and imageBase64Data
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
                MemoryStream memoryStream = new(); // Initializes memoryStream of type MemoryStream
                await file.CopyToAsync(memoryStream); // Uploads file content to memory stream to be backed up and stored
                byte[] byteFile = memoryStream.ToArray(); // Turns the stream to a byte array
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
            decimal fileSize = bytes;

            while (Math.Round(fileSize / 1024) >= 1)
            {
                fileSize /= bytes; // Divides fileSize by bytes through every loop
                counter++; // Adds 1 to counter through every loop
            }

            return String.Format("{0:n1}{1}", fileSize, suffixes[counter]); // Returns a string formatted with the fileSize and suffixes[counter]
        }
        #endregion

        #region GetFileIcon
        public string GetFileIcon(string file)
        {
            string fileImage = "default";

            if (!string.IsNullOrWhiteSpace(file)) // If string is not null or white space
            {
                fileImage = Path.GetExtension(file).Replace(".", ""); // Replaces all . with an empty space and returns the file image path
                return $"/img/png/{fileImage}.png";
            }

            return fileImage;
        } 
        #endregion
    }
}
