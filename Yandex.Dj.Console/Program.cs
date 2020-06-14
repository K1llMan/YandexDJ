using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Common.YTrack;
using Yandex.Music.Api.Models.Artist;

namespace Yandex.Dj.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            YAuthStorage storage = new YAuthStorage();
            YandexMusicApi api = new YandexMusicApi();
            api.UserAPI.Authorize(storage, "AgAAAAACTtHDAAG8XqpUsgWDJ0ukuj0pF2mwlnY");

            var searchTrack = api.SearchAPI.Suggest(storage, "Arven");

            var artist = api.ArtistAPI.Get(storage, "3121");
            //var playlist = api.PlaylistAPI.Get(storage, "139954184", "2050");
            //var added = api.LibraryAPI.AddPlaylistLike(storage, playlist);
            //var removed = api.LibraryAPI.RemovePlaylistLike(storage, playlist);

            //var searchArtist = api.SearchAPI.Artist(storage, "Arven").Artists.Results.First();
            //var artist = new YArtist {
            //    Id = searchArtist.Id
            //};
            //var added = api.LibraryAPI.AddArtistLike(storage, artist);
            //var removed = api.LibraryAPI.RemoveArtistLike(storage, artist);

            //var searchTrack = api.SearchAPI.Track(storage, "Arven - All I Got").Tracks.Results.First();
            //var track = new YTrack {
            //    Id = searchTrack.Id,
            //    Albums = searchTrack.Albums.Select(a => new YAlbum {
            //        Id = a.Id
            //    }).ToList()
            //};

            //var revision = api.LibraryAPI.AddTrackLike(storage, track);
            //revision = api.LibraryAPI.RemoveTrackLike(storage, track);
            //var revision = api.LibraryAPI.AddTrackDislike(storage, track);
            //revision = api.LibraryAPI.RemoveTrackDislike(storage, track);

            //var albumResponse = api.AlbumAPI.Get(storage, "621147");
            //var album = new YAlbum {
            //    Id = albumResponse.Id
            //};
            //var added = api.LibraryAPI.AddAlbumLike(storage, album);
            //var removed = api.LibraryAPI.RemoveAlbumLike(storage, album);

            //var likedTracks = api.LibraryAPI.GetLikedTracks(storage);
            //var dislikedTracks = api.LibraryAPI.GetDislikedTracks(storage);
            //var likedAlbums = api.LibraryAPI.GetLikedAlbums(storage);
            //var likedArtists = api.LibraryAPI.GetLikedArtists(storage);
            //var likedPlaylists = api.LibraryAPI.GetLikedPlaylists(storage);

            //api.TrackAPI.ExtractToFile(storage, "106259", "C:\\test.mp3");
            //var response2 = api.TrackAPI.GetMetadataForDownload(storage, "106259");
            //var response12 = api.TrackAPI.GetDownloadFileInfo(storage, response2.First(m => m.Codec == "mp3"));
            //var response1 = api.TrackAPI.Get(storage, "106259");
            //var response8 = api.PlaylistAPI.Create(storage, "test1234");
            //var response7 = api.PlaylistAPI.Rename(storage, response8, "test1234");
            //

            //var track = api.TrackAPI.Get(storage, searchTrack.Id);
            //var response20 = api.PlaylistAPI.InsertTracks(storage, response7, new List<YTrack> {
            //    track
            //});
            //
            //var response10 = api.PlaylistAPI.Get(storage, storage.User.Uid, response8.Kind);
            //
            //List<YTrack> tracks = new List<YTrack> { response10.Tracks[0].Track };
            //
            //var response11 = api.PlaylistAPI.DeleteTrack(storage, response10, tracks);

            //var response9 = api.PlaylistAPI.Delete(storage, response8.Kind);
            //var response6 = api.PlaylistAPI.DejaVu(storage);
            //var response5 = api.PlaylistAPI.Get(storage, "139954184", "2050");
            //var response4 = api.PlaylistAPI.Favorites(storage);
            //var response3 = api.PlaylistAPI.MainPagePersonal(storage);
            //var response100500 = api.SearchAPI.Albums(storage, "The Power Within");
        }
    }
}
