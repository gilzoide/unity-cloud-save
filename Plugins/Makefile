OBJCFLAGS += -arch arm64 -arch x86_64 -fobjc-arc
CODESIGN ?= codesign
MACOS_CODESIGN_SIGNATURE ?=

Apple/libgilzoide.cloudsave.gamecenter.dylib: LINKFLAGS += -shared -framework GameKit
Apple/libgilzoide.cloudsave.gamecenter.dylib: Apple/GameCenterCloudSave.m
	$(CC) $< -o $@ $(OBJCFLAGS) $(LINKFLAGS)
ifdef MACOS_CODESIGN_SIGNATURE
	$(CODESIGN) -s "$(MACOS_CODESIGN_SIGNATURE)" $@
endif

gamecenter: Apple/libgilzoide.cloudsave.gamecenter.dylib
