﻿using EA4S.MinigamesAPI;

namespace EA4S.MinigamesCommon
{
    /// <summary>
    /// Interface for all configuration containers of minigames.
    /// Holds data passed from the core application to the minigame at a given instance.
    /// All minigames define their own configuration class.
    /// </summary>
    public interface IGameConfiguration
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        IGameContext Context { get; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        IQuestionProvider Questions { get; set; }

        /// <summary>
        /// Return the builder that defines the rules to build question packs
        /// </summary>
        /// <returns></returns>
        IQuestionBuilder SetupBuilder();

        /// <summary>
        /// Return the rules for learning related to this minigame
        /// </summary>
        /// <returns></returns>
        MiniGameLearnRules SetupLearnRules();

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        /// <value>
        /// The difficulty.
        /// </value>
        float Difficulty { get; set; }
    }
}