using IpisCentralDisplayController.Models;
using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using IpisCentralDisplayController.models;
using System.Diagnostics;

namespace IpisCentralDisplayController.Managers
{
    public class MediaManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _mediaFilesKey = "mediaFiles";

        public MediaManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<MediaFile> LoadMediaFiles()
        {
            return _jsonHelper.Load<List<MediaFile>>(_mediaFilesKey) ?? new List<MediaFile>();
        }

        //public void SaveMediaFiles(List<MediaFile> mediaFiles)
        //{
        //    _jsonHelper.Save(_mediaFilesKey, mediaFiles);
        //}

        public void SaveMediaFiles(List<MediaFile> mediaFiles)
        {
            try
            {
                // Log the media files being saved
                Debug.WriteLine($"Saving {mediaFiles.Count} media files.");
                foreach (var mediaFile in mediaFiles)
                {
                    Debug.WriteLine($"MediaFile - Id: {mediaFile.Id}, Name: {mediaFile.Name}, FilePath: {mediaFile.FilePath}, Created: {mediaFile.Created}, Updated: {mediaFile.Updated}");
                }

                _jsonHelper.Save(_mediaFilesKey, mediaFiles);
            }
            catch (Exception ex)
            {
                // Log or handle the serialization error
                Debug.WriteLine($"Error saving media files: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public void AddMediaFile(MediaFile mediaFile)
        {
            var mediaFiles = LoadMediaFiles();
            if (mediaFiles.Any(m => m.Id == mediaFile.Id))
            {
                //throw new Exception("Media file with this ID already exists.");
            }
            else
            {
                mediaFiles.Add(mediaFile);
                SaveMediaFiles(mediaFiles);
            }
        }

        public void UpdateMediaFile(MediaFile mediaFile)
        {
            var mediaFiles = LoadMediaFiles();
            var existingMediaFile = mediaFiles.FirstOrDefault(m => m.Id == mediaFile.Id);
            if (existingMediaFile == null)
            {
                throw new Exception("Media file not found.");
            }
            // Update media file properties here
            existingMediaFile.Name = mediaFile.Name;
            existingMediaFile.FilePath = mediaFile.FilePath;
            existingMediaFile.Updated = DateTime.Now;
            // If specific properties for each type need to be updated, handle them here

            SaveMediaFiles(mediaFiles);
        }

        public void DeleteMediaFile(string id)
        {
            var mediaFiles = LoadMediaFiles();
            var mediaFile = mediaFiles.FirstOrDefault(m => m.Id == id);
            if (mediaFile == null)
            {
                throw new Exception("Media file not found.");
            }
            mediaFiles.Remove(mediaFile);
            SaveMediaFiles(mediaFiles);
        }

        public void DeleteAllMediaFiles()
        {
            var mediaFiles = LoadMediaFiles();
            mediaFiles.Clear();
            SaveMediaFiles(mediaFiles);
        }

        public MediaFile FindMediaFileById(string id)
        {
            var mediaFiles = LoadMediaFiles();
            return mediaFiles.FirstOrDefault(m => m.Id == id);
        }

        public List<ImageFile> GetImageFiles()
        {
            return LoadMediaFiles().OfType<ImageFile>().ToList();
        }

        public List<VideoFile> GetVideoFiles()
        {
            return LoadMediaFiles().OfType<VideoFile>().ToList();
        }

        public List<TextSlideFile> GetTextSlideFiles()
        {
            return LoadMediaFiles().OfType<TextSlideFile>().ToList();
        }

        public List<AudioFile> GetAudioFiles()
        {
            return LoadMediaFiles().OfType<AudioFile>().ToList();
        }
    }
}
