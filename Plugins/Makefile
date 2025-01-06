OBJCFLAGS += -arch arm64 -arch x86_64 -fobjc-arc
CODESIGN ?= codesign
MACOS_CODESIGN_SIGNATURE ?=

Apple/libgilzoide.cloudsave.apple.dylib: LINKFLAGS += -shared -framework GameKit
Apple/libgilzoide.cloudsave.apple.dylib: Apple/AppleCloudSave.m
	$(CC) $< -o $@ $(OBJCFLAGS) $(LINKFLAGS)
ifdef MACOS_CODESIGN_SIGNATURE
	$(CODESIGN) -s "$(MACOS_CODESIGN_SIGNATURE)" $@
endif

gamecenter: Apple/libgilzoide.cloudsave.apple.dylib
