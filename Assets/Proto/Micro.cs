// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: micro.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Rpc {

  /// <summary>Holder for reflection information generated from micro.proto</summary>
  public static partial class MicroReflection {

    #region Descriptor
    /// <summary>File descriptor for micro.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MicroReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgttaWNyby5wcm90bxIDcnBjKjoKCE1pY3JvUlBDEhIKDk1JQ1JPX1JQQ19O",
            "T05FEAASDAoIUkVRX0NIQVQQARIMCghBQ0tfQ0hBVBACYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Rpc.MicroRPC), }, null, null));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  /// GameLobbyRPC RPC 15000 ~ 16000
  /// </summary>
  public enum MicroRPC {
    [pbr::OriginalName("MICRO_RPC_NONE")] None = 0,
    [pbr::OriginalName("REQ_CHAT")] ReqChat = 1,
    [pbr::OriginalName("ACK_CHAT")] AckChat = 2,
  }

  #endregion

}

#endregion Designer generated code
