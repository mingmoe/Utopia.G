// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Transition;

public static class Utility
{
    public static ITranslatedString Activate(this TranslateManager manager,
        TranslateKey key,
        TranslateIdentifence id,
        object data) => new ICUTranslatedString(key, manager, id, data);
}
