// Flexlive.CQP.CQCPlugin.h

#pragma once

//#define CQAPPID "Flexlive.CQP.CQCPlugin" //���޸�AppID������� http://d.cqp.me/Pro/����/������Ϣ
//#define CQAPPINFO CQAPIVERTEXT "," CQAPPID

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;


namespace FlexliveCQP {

	public ref class CQCPlugin
	{
	private:
		static Object^ CQNaviteProxyObject;
		static MethodInfo^ CQNaviteProxyMethod;

	public:
		static void Initialize(String^ appDirectory);
		static void SendMessage(String^ message);
		static void CSharpTrans(String^ eventType, int subType, int sendTime, long fromGroup, long fromDiscuss,
			long fromQQ, String^ fromAnonymous, long beingOperateQQ, String^ msg, int font, String^ responseFlag, String^ file);
	};
}
