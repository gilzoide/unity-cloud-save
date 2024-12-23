#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import <string.h>

static NSString *toNSString(const char *s) {
	return [NSString stringWithCString:s encoding:NSUTF8StringEncoding];
}

static const char *safestrdup(const char *s) {
	if (s) {
		return strdup(s);
	}
	else {
		return NULL;
	}
}

@interface GCSProvider : NSObject<GKLocalPlayerListener>

@property(retain) NSArray<GKSavedGame *> *savedGames;

+ (GCSProvider *)sharedInstance;

- (void)fetchGamesUsingCache:(BOOL)useCache withCallback:(void(^)(NSArray<GKSavedGame *> *savedGames, NSError *error))callback;
- (void)loadGameNamed:(NSString *)name withCallback:(void(^)(GKSavedGame *savedGame, NSError *error))callback;
- (void)saveGameData:(NSData *)data withName:(NSString *)name withCallback:(void(^)(GKSavedGame *savedGame, NSError *error))callback;
- (void)deleteGamesNamed:(NSString *)name withCallback:(void(^)(NSError *error))callback;

// GKSavedGameListener
- (void)player:(GKPlayer *)player hasConflictingSavedGames:(NSArray<GKSavedGame *> *)savedGames;

@end

@implementation GCSProvider

+ (GCSProvider *)sharedInstance {
	static GCSProvider *instance = nil;
	static dispatch_once_t token;
	dispatch_once(&token, ^{
		instance = [[self alloc] init];
	});
	return instance;
}

- (instancetype)init {
	if (self = [super init]) {
		[GKLocalPlayer.localPlayer registerListener:self];
	}
	return self;
}

- (void)dealloc {
	[GKLocalPlayer.localPlayer unregisterListener:self];
}

- (void)fetchGamesUsingCache:(BOOL)useCache withCallback:(void(^)(NSArray<GKSavedGame *> *savedGames, NSError *error))callback {
	if (useCache && self.savedGames) {
		if (callback) callback(self.savedGames, nil);
	}
	else {
		[GKLocalPlayer.localPlayer fetchSavedGamesWithCompletionHandler:^(NSArray<GKSavedGame *> *savedGames, NSError *error) {
			self.savedGames = savedGames;
			if (callback) callback(savedGames, error);
		}];
	}
}

- (void)loadGameNamed:(NSString *)name withCallback:(void(^)(GKSavedGame *savedGame, NSError *error))callback; {
	[self fetchGamesUsingCache:YES withCallback:^(NSArray<GKSavedGame *> *savedGames, NSError *error) {
		if (error) {
			if (callback) callback(nil, error);
		}
		else {
			for (GKSavedGame *game in savedGames) {
				if ([game.name isEqualToString:name]) {
					if (callback) callback(game, nil);
					return;
				}
			}
			if (callback) callback(nil, nil);
		}
	}];
}

- (void)saveGameData:(NSData *)data withName:(NSString *)name withCallback:(void(^)(GKSavedGame *savedGame, NSError *error))callback {
	[GKLocalPlayer.localPlayer saveGameData:data withName:name completionHandler:^(GKSavedGame *savedGame, NSError *error) {
		if (savedGame) {
			// refresh saved games array if saved game
			[self fetchGamesUsingCache:NO withCallback:nil];
		}
		if (callback) callback(savedGame, error);
	}];
}

- (void)deleteGamesNamed:(NSString *)name withCallback:(void(^)(NSError *error))callback {
	[GKLocalPlayer.localPlayer deleteSavedGamesWithName:name completionHandler:callback];
}

// GKSavedGameListener
- (void)player:(GKPlayer *)player hasConflictingSavedGames:(NSArray<GKSavedGame *> *)savedGames {
	GKSavedGame *newestGame = nil;
	for (GKSavedGame *game in savedGames) {
		if (newestGame == nil || [game.modificationDate timeIntervalSinceDate:newestGame.modificationDate] > 0) {
			newestGame = game;
		}
	}
	[newestGame loadDataWithCompletionHandler:^(NSData *data, NSError *error) {
		[GKLocalPlayer.localPlayer resolveConflictingSavedGames:savedGames withData:data completionHandler:^(NSArray<GKSavedGame *> *savedGames, NSError *error) {}];
	}];
}

@end

///////////////////////////////////////////////////////////
// Exported functions
///////////////////////////////////////////////////////////

// GameCenterCloudSaveProvider
bool Gilzoide_CloudSave_GameCenter_IsEnabled() {
	return GKLocalPlayer.localPlayer.authenticated;
}

void Gilzoide_CloudSave_GameCenter_Fetch(void (*callback)(void *userdata, NSArray<GKSavedGame *> *savedGames, NSError *error), void *userdata) {
	[GCSProvider.sharedInstance fetchGamesUsingCache:NO withCallback:^(NSArray<GKSavedGame *> *savedGames, NSError *error) {
		callback(userdata, savedGames, error);
	}];
}

void Gilzoide_CloudSave_GameCenter_Load(const char *name, void (*callback)(void *userdata, GKSavedGame *savedGame, NSError *error), void *userdata) {
	[GCSProvider.sharedInstance loadGameNamed:toNSString(name) withCallback:^(GKSavedGame *savedGame, NSError *error) {
		callback(userdata, savedGame, error);
	}];
}

void Gilzoide_CloudSave_GameCenter_Save(const char *name, void *bytes, long bytesSize, void (*callback)(void *userdata, GKSavedGame *savedGame, NSError *error), void *userdata) {
	NSData *data = [NSData dataWithBytes:bytes length:(NSUInteger)bytesSize];
	[GCSProvider.sharedInstance saveGameData:data withName:toNSString(name) withCallback:^(GKSavedGame *savedGame, NSError *error) {
		callback(userdata, savedGame, error);
	}];
}

void Gilzoide_CloudSave_GameCenter_Delete(const char *name, void (*callback)(void *userdata, NSError *error), void *userdata) {
	[GCSProvider.sharedInstance deleteGamesNamed:toNSString(name) withCallback:^(NSError *error) {
		callback(userdata, error);
	}];
}

// GKSavedGameRef
const char *Gilzoide_CloudSave_GameCenter_SavedGameName(GKSavedGame *savedGame) {
	return safestrdup(savedGame.name.UTF8String);
}

void Gilzoide_CloudSave_GameCenter_SavedGameLoad(GKSavedGame *savedGame, void(*callback)(void *userdata, NSData *data, NSError *error), void *userdata) {
	[savedGame loadDataWithCompletionHandler:^(NSData *data, NSError *error) {
		callback(userdata, data, error);
	}];
}

int64_t Gilzoide_CloudSave_GameCenter_SavedGameLastModifiedTimestampUnix(GKSavedGame *savedGame) {
	return (int64_t) savedGame.modificationDate.timeIntervalSince1970;
}

// CFErrorRef
const char *Gilzoide_CloudSave_GameCenter_ErrorToString(NSError *error) {
	return safestrdup(error.localizedDescription.UTF8String);
}
