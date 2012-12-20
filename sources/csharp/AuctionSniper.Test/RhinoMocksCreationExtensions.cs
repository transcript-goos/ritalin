using System;

using Rhino.Mocks;

namespace Test {
    internal static class RhinoMocksCreationExtensions {
        /// <summary>Generates a stub without needing a <see cref="MockRepository"/></summary>
        /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
        /// <typeparam name="T">The <see cref="Type"/> of stub to create.</typeparam>
        /// <returns>The stub</returns>
        /// <seealso cref="Stub{T}"/>
        public static T GenerateStubHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) where T : class {
            return CreateMockInReplay(repository, repo => (T)repo.Stub(typeof(T), argumentsForConstructor));
        }
        
        /// <summary>Generates a stub without needing a <see cref="MockRepository"/></summary>
        /// <param name="type">The <see cref="Type"/> of stub.</param>
        /// <param name="argumentsForConstructor">Arguments for the <paramref name="type"/>'s constructor.</param>
        /// <returns>The stub</returns>
        /// <seealso cref="Stub"/>
        public static object GenerateStubHelper(this MockRepository repository, Type type, params object[] argumentsForConstructor) {
            return CreateMockInReplay(repository, repo => repo.Stub(type, argumentsForConstructor));
        }
        
        /// <summary>Generate a mock object without needing a <see cref="MockRepository"/></summary>
        /// <typeparam name="T">type <see cref="Type"/> of mock object to create.</typeparam>
        /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
        /// <returns>the mock object</returns>
        /// <seealso cref="DynamicMock{T}"/>
        public static T GenerateMockHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) where T : class {
            return CreateMockInReplay(repository, r => r.DynamicMock<T>(argumentsForConstructor));
        }
        
        /// <summary>Generate a multi-mock object without needing a <see cref="MockRepository"/></summary>
        /// <typeparam name="T">The <c>typeof</c> object to generate a mock for.</typeparam>
        /// <typeparam name="TMultiMockInterface1">A second interface to generate a multi-mock for.</typeparam>
        /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
        /// <returns>the multi-mock object</returns>
        /// <seealso cref="DynamicMultiMock(System.Type,System.Type[],object[])"/>
        public static T GenerateMockHelper<T, TMultiMockInterface1>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GenerateMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1) }, argumentsForConstructor);
        }
        
        /// <summary>Generate a multi-mock object without without needing a <see cref="MockRepository"/></summary>
        /// <typeparam name="T">The <c>typeof</c> object to generate a mock for.</typeparam>
        /// <typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for.</typeparam>
        /// <typeparam name="TMultiMockInterface2">A second interface to generate a multi-mock for.</typeparam>
        /// <param name="argumentsForConstructor">Arguments for <typeparamref name="T"/>'s constructor</param>
        /// <returns>the multi-mock object</returns>
        /// <seealso cref="DynamicMultiMock(Type,Type[],object[])"/>
        public static T GenerateMockHelper<T, TMultiMockInterface1, TMultiMockInterface2>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GenerateMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1), typeof(TMultiMockInterface2) }, argumentsForConstructor
            );
        }
        
        /// <summary>Creates a multi-mock without without needing a <see cref="MockRepository"/></summary>
        /// <param name="type">The type of mock to create, this can be a class</param>
        /// <param name="extraTypes">Any extra interfaces to add to the multi-mock, these can only be interfaces.</param>
        /// <param name="argumentsForConstructor">Arguments for <paramref name="type"/>'s constructor</param>
        /// <returns>the multi-mock object</returns>
        /// <seealso cref="DynamicMultiMock(System.Type,System.Type[],object[])"/>
        public static object GenerateMockHelper(this MockRepository repository, Type type, Type[] extraTypes, params object[] argumentsForConstructor) {
            return CreateMockInReplay(repository, r => r.DynamicMultiMock(type, extraTypes, argumentsForConstructor));
        }
        
        ///<summary>Creates a strict mock without without needing a <see cref="MockRepository"/></summary>
        ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
        ///<typeparam name="T">The type of mock object to create.</typeparam>
        ///<returns>The mock object with strict replay semantics</returns>
        /// <seealso cref="StrictMock{T}"/>
        public static T GenerateStrictMockHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) {
            return CreateMockInReplay(repository, r => r.StrictMock<T>(argumentsForConstructor));
        }
        
        ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
        ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
        ///<typeparam name="T">The type of mock object to create, this can be a class.</typeparam>
        ///<typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for, this must be an interface!</typeparam>
        ///<returns>The multi-mock object with strict replay semantics</returns>
        /// <seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
        public static T GenerateStrictMockHelper<T, TMultiMockInterface1>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GenerateStrictMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1) }, argumentsForConstructor
            );
        }
        
        ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
        ///<param name="argumentsForConstructor">Any arguments required for the <typeparamref name="T"/>'s constructor</param>
        ///<typeparam name="T">The type of mock object to create, this can be a class.</typeparam>
        ///<typeparam name="TMultiMockInterface1">An interface to generate a multi-mock for, this must be an interface!</typeparam>
        ///<typeparam name="TMultiMockInterface2">A second interface to generate a multi-mock for, this must be an interface!</typeparam>
        ///<returns>The multi-mock object with strict replay semantics</returns>
        ///<seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
        public static T GenerateStrictMockHelper<T, TMultiMockInterface1, TMultiMockInterface2>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GenerateStrictMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1), typeof(TMultiMockInterface2) }, argumentsForConstructor
            );
        }
        
        ///<summary>Creates a strict multi-mock without needing a <see cref="MockRepository"/></summary>
        ///<param name="type">The type of mock object to create, this can be a class</param>
        ///<param name="extraTypes">Any extra interfaces to generate a multi-mock for, these must be interaces!</param>
        ///<param name="argumentsForConstructor">Any arguments for the <paramref name="type"/>'s constructor</param>
        ///<returns>The strict multi-mock object</returns>
        /// <seealso cref="StrictMultiMock(System.Type,System.Type[],object[])"/>
        public static object GenerateStrictMockHelper(this MockRepository repository, Type type, Type[] extraTypes, params object[] argumentsForConstructor) {
            if (extraTypes == null) extraTypes = new Type[0];
            if (argumentsForConstructor == null) argumentsForConstructor = new object[0];
            
            return CreateMockInReplay(repository, r => r.StrictMultiMock(type, extraTypes, argumentsForConstructor));
        }
        
        ///<summary>
        ///</summary>
        ///<param name="argumentsForConstructor"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public static T GeneratePartialMockHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GeneratePartialMockHelper(
                repository,
                typeof(T), new Type[0], argumentsForConstructor
                );
        }
        
        ///<summary>
        ///</summary>
        ///<param name="argumentsForConstructor"></param>
        ///<typeparam name="T"></typeparam>
        ///<typeparam name="TMultiMockInterface1"></typeparam>
        ///<returns></returns>
        public static T GeneratePartialMockHelper<T, TMultiMockInterface1>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GeneratePartialMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1) }, argumentsForConstructor
            );
        }
        
        ///<summary>
        ///</summary>
        ///<param name="argumentsForConstructor"></param>
        ///<typeparam name="T"></typeparam>
        ///<typeparam name="TMultiMockInterface1"></typeparam>
        ///<typeparam name="TMultiMockInterface2"></typeparam>
        ///<returns></returns>
        public static T GeneratePartialMockHelper<T, TMultiMockInterface1, TMultiMockInterface2>(this MockRepository repository, params object[] argumentsForConstructor) {
            return (T)RhinoMocksCreationExtensions.GeneratePartialMockHelper(
                repository,
                typeof(T), new Type[] { typeof(TMultiMockInterface1), typeof(TMultiMockInterface2) }, argumentsForConstructor
            );
        }
        
        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<param name="extraTypes"></param>
        ///<param name="argumentsForConstructor"></param>
        ///<returns></returns>
        public static object GeneratePartialMockHelper(this MockRepository repository, Type type, Type[] extraTypes, params object[] argumentsForConstructor) {
            return CreateMockInReplay(repository, r => r.PartialMultiMock(type, extraTypes, argumentsForConstructor));
        }
        
        /// <summary>
        /// Generate a mock object with dynamic replay semantics and remoting without needing the mock repository
        /// </summary>
        public static T GenerateDynamicMockWithRemotingHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) {
            return CreateMockInReplay(repository, r => r.DynamicMockWithRemoting<T>(argumentsForConstructor));
        }
        
        /// <summary>
        /// Generate a mock object with strict replay semantics and remoting without needing the mock repository
        /// </summary>
        public static T GenerateStrictMockWithRemotingHelper<T>(this MockRepository repository, params object[] argumentsForConstructor) where T : class {
            return CreateMockInReplay(repository, r => r.StrictMockWithRemoting<T>(argumentsForConstructor));
        }
        
        private static T CreateMockInReplay<T>(MockRepository repository, Func<MockRepository, T> createMock) {
            var mockObject = createMock(repository);
            repository.Replay(mockObject);
            
            return mockObject;
        }
    }
}