# Builds the Realm .NET native wrappers as a static library for the browser (Emscripten).
#
# This mirrors the upstream realm-dotnet wrappers configuration (https://github.com/realm/realm-dotnet),
# which only supports building a shared library.
#
# Note that Realm Core only supports single-threaded WebAssembly, and keeps databases in memory when built for Emscripten.

cmake_minimum_required(VERSION 3.15)

project(realm-wrappers-browser C CXX)

set(REALM_DOTNET_SOURCE_DIR "" CACHE PATH "Path to the realm-dotnet source")

if(NOT EXISTS "${REALM_DOTNET_SOURCE_DIR}/wrappers/realm-core/CMakeLists.txt")
    message(FATAL_ERROR "REALM_DOTNET_SOURCE_DIR must point to a realm-dotnet checkout with initialised submodules.")
endif()

# Realm's Emscripten support requires fixes which haven't been made upstream:
# - Change notifications are deferred to the event loop, as running them from within a commit breaks invariants.
# - The in-memory database is initialised from the database file at the configured path (if present), to allow for persistence.
# - Realm instances are not cached, as the .NET scheduler considers all instances on the same thread to be equivalent.
execute_process(
    COMMAND git apply "${CMAKE_CURRENT_SOURCE_DIR}/emscripten.patch"
    WORKING_DIRECTORY "${REALM_DOTNET_SOURCE_DIR}/wrappers/realm-core"
    OUTPUT_QUIET
    ERROR_QUIET
)
execute_process(
    COMMAND git apply "${CMAKE_CURRENT_SOURCE_DIR}/emscripten-wrappers.patch"
    WORKING_DIRECTORY "${REALM_DOTNET_SOURCE_DIR}"
    OUTPUT_QUIET
    ERROR_QUIET
)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(REALM_BUILD_LIB_ONLY ON)
set(REALM_ENABLE_ASSERTIONS ON CACHE BOOL "Enable release assertions")

add_subdirectory("${REALM_DOTNET_SOURCE_DIR}/wrappers/realm-core" realm-core EXCLUDE_FROM_ALL)

file(GLOB REALM_WRAPPERS_SOURCES "${REALM_DOTNET_SOURCE_DIR}/wrappers/src/*.cpp")

add_library(realm-wrappers STATIC ${REALM_WRAPPERS_SOURCES})

target_compile_options(realm-wrappers PRIVATE -Wno-missing-prototypes)
target_link_libraries(realm-wrappers Realm::ObjectStore Realm::QueryParser)
