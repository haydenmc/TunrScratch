export interface ITrackModel {
    // IDs
    trackId: string;
    userId: string;
    // Audio info
    audioChannels: number;
    audioBitrateKbps: number;
    audioSampleRateHz: number;
    audioDurationSeconds: number;
    // Tags
    tagTitle: number;
    tagPerformers: string[];
    tagAlbumArtist: string;
    tagAlbum: string;
    tagComposers: string[];
    tagGenres: string[];
    tagComment: string;
    tagYear: number;
    tagBeatsPerMinute: number;
    tagTrackNumber: number;
    tagAlbumTrackCount: number;
    tagDiscNumber: number;
    tagAlbumDiscCount: number;
    // Library info
    libraryPlays: number;
    libraryRating: number;
    libraryDateTimeAdded: number;
    libraryDateTimeModified: number;
}
