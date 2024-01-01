cmake_minimum_required (VERSION 3.27)

# import share cmake script
include("${CMAKE_CURRENT_LIST_DIR}/../share.cmake")

project ("Utopia.Kcp" VERSION "${READ_UTOPIA_VERSION}" LANGUAGES C)

add_library("Utopia.Kcp" SHARED "${CMAKE_CURRENT_SOURCE_DIR}/kcp/ikcp.c")

if (MSVC)
    target_compile_options("Utopia.Kcp" PUBLIC /w /std:c11 /utf-8 /O2 /permissive-)
else()
    target_compile_options("Utopia.Kcp" PUBLIC -w -O3 -std=c11)
endif()

target_compile_definitions("Utopia.Kcp" PUBLIC "NDEBUG" "NODEBUG")
