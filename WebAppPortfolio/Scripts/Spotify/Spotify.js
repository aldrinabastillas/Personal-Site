angular.module('spotifyApp', [])
    .controller('SpotifyController', ['$scope', function ($scope) {
        var spotify = this;

        spotify.Search = function (searchTerm) {
            var id = searchTerm
            $.getJSON('GetSpotifyAudioFeaturesAsync/' + id, function (response) {

            });
        };
    }]);