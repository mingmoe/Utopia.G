
# force release
set(CMAKE_BUILD_TYPE "Release")

# read version
set(READ_UTOPIA_VERSION "1.0.0")

# set output directory
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_CURRENT_LIST_DIR}/precompiled-library")
file(MAKE_DIRECTORY ${CMAKE_RUNTIME_OUTPUT_DIRECTORY})
